# basic imports
import datetime
import asyncio

from Core.Application.Acquire import Acquire
from Core.Application.Process import Process
from Core.Application.Handover import Handover
from Core.Entities.PipeElement import PipeElement
from Exchanges.NasdaqOmxClient import NasdaqOmxClient
from Exchanges.XetraClient import XetraClient
from Gateways.ElisGraphQlClient import ElisCqlClient
from Gateways.CqlQueryBuilder import CqlStocksQueryBuilder
from Gateways.CqlStockMutationBuilder import CqlStockMutationBuilder

# TODO Restructure for pipes and operations.
# TODO
# TODO To get stock data from an exchange the following operations have to be executed:
# TODO   1) Acquire json data for the stock,
# TODO   2) Process json data, and
# TODO   3) Store data in Elis.
# TODO The list of stocks to get data for is acquired from Elis.
# TODO The operations are in detail as follows:
# TODO Operation 1 gets the series of prize and volume from the specific exchange for the stock.
# TODO             The operation returns a generator, which generates a prize and volume tuple read in
# TODO             the response from the exchange.
# TODO Operation 2 creates a GraphQL mutation of the acquired prizes and volumes using the generator.
# TODO             The operation returns text holding the mutation of new stock data.
# TODO Operation 3 hand the mutation over to Elis using the GraphQL interface.
# TODO The operations are executed in sequence in an iteration over the list of stocks.
# TODO The iteration loop passes a pipe element to each operation. The pipe element is the same
# TODO structure that holds stock, generator, mutation and status, where:
# TODO   - Stock is the stock under process,
# TODO   - Generator is the resulting generator of prize and volume tuples,
# TODO   - Mutation is the GraphQL mutation query, and
# TODO   - Status holds the resulting status of each operation. It may hold a boolean that is true,
# TODO     when the operation succeeded otherwise false and a text for the status like an error message.
# TODO
# TODO Each operation enriches the pipe element with the operation result using the results from previous
# TODO operational steps. The next operation is executed, when the status of the execution of the previous
# TODO operation was succesfull. The operation calls the specific service to execute based
# TODO on data in the pipe element. For example in operation 1 is the specific web-api used for
# TODO the exchange that has the stock data. And a specific generator has also to be created. This can
# TODO be done by services registered in operation 1 at startup from a configuration. The exchange property
# TODO is on the stock object acquired from Elis.
# TODO
# TODO The construction has to be SOLID. Each operation and service has a single responsibility.
# TODO The operations and services are open to extensions and closed for modifications.
# TODO The services used in an operation have to derive from a base class declaring the common interface
# TODO for the services used in the specific operation. The registering of services means injection
# TODO and dependency inversion is implemented. Thus i and d in solid.
#

# TODO where has this date helper functions to go?
def getExchangeDay( exchangeDay ):
    while exchangeDay.weekday() > 4 :
        exchangeDay -= datetime.timedelta(days=1)
    return exchangeDay

# TODO set configuration from file or arguments
elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
url = 'http://localhost:54676/graphql'
 # TODO not in nasdaq format
fromDate = datetime.datetime.strptime( '2020-01-01', '%Y-%m-%d')
toDate = getExchangeDay(datetime.datetime.now() )
saveMutationInCqlFile = False
# Stock filter for the stocks to get data for
isin = ''  #'IE00B5BMR087'
exchange = '' #'XETR'

elisCqlClient = ElisCqlClient(url) # TODO ???

def buildPipeline():
    operations = []
    # Build exchange clients
    acquire = Acquire()
    nasdaqOmxClient = NasdaqOmxClient(elisDateTimeFormat)
    acquire.Register(nasdaqOmxClient, 'XCSE')
    xetraClient = XetraClient(elisDateTimeFormat)
    acquire.Register(xetraClient, 'XETR')
    operations.append(acquire)
    # Build mutation processor handler
    process = Process()
    stockMutationBuilder = CqlStockMutationBuilder()
    process.Register(stockMutationBuilder)
    operations.append(process)
    # Build handover handler
    handover = Handover()
    handover.Register(elisCqlClient)
    operations.append(handover)
    return operations

async def saveInCqlFile( name, data):
    fileName = name + '_Cql.txt'
    with open(fileName, "w") as txtfile:
        txtfile.write(data)

async def main():
    # Construct pipeline and register handlers in operations.
    pipeline = buildPipeline()
    # Acquire list of stocks
    stockQueryBuilder = CqlStocksQueryBuilder() # TODO Is a class necessary?
    query = stockQueryBuilder.buildCqlStocksQuery(isin, exchange)
    stocks = await elisCqlClient.getStocks( query)
    # Iterate through the list executing operations in sequences
    for stock in stocks:
        pipeElement = PipeElement(fromDate, toDate, stock)        
        # report working on stock ...
        print('Gets stock data for ' + pipeElement.stock.name)
        for operation in pipeline:
            result = await operation.Execute(pipeElement)
            # report operation done with status
            if not result.status.status:
                break
            pipeElement = result
        # Save mutation 
        if saveMutationInCqlFile:
            print('Save GraphQL mutation for ' + pipeElement.stock.name)
            await saveInCqlFile( pipeElement.stock.isin, pipeElement.mutation)
        # destruct pipeElement for garbage collection
        del pipeElement
    # report done

asyncio.run(main())