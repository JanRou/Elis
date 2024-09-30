# basic imports
import time
import json
import datetime

# http communication
import requests

# GraphQl client
import asyncio
from gql import Client, gql
from gql.transport.httpx import HTTPXAsyncTransport

def getStockPrizesFromNasdaqNordic(instrument, isin, name, fromDay, toDay):
    ## konsolidatorUrlKurser='http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE172620&name=Konsolidator&ISIN=DK0061113511'
    ## GAMLE konsolidatorUrl='http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=20200415&ToDate=20210416&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE172620'
    ## NYE konsolidatorUrl='https://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=1986-01-01&ToDate=2024-06-25&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE172620'
    ## 
    # https://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE172620&name=Konsolidator&ISIN=DK0061113511
    url  = 'http://www.nasdaqomxnordic.com/aktier/microsite?'
    url += 'Instrument=' + instrument
    url += '&name=' + name
    url += '&ISIN=' + isin

    quoteUrl  = 'http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&'
    quoteUrl += 'FromDate=' + fromDay
    quoteUrl += '&ToDate=' + toDay
    quoteUrl += '&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&'
    quoteUrl += 'Instrument=' + instrument

    result = []
    header = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0'
            , 'Accept-Language': 'da,en-US;q=0.7,en;q=0.3'
            , 'Accept-Encoding': 'text/html, */*; q=0.01'
            , 'Connection': 'keep-alive'
            , 'Accept': 'application/json, text/javascript, */*; q=0.01'
            }
    session = requests.Session();
    session.headers = header;

    resp = session.get(url)
    time.sleep(5)

    session.headers.update({'X-Requested-With': 'XMLHttpRequest'})
    session.headers.update({'Content-Type':	'application/json; charset=UTF-8'})
    print('Get prizes for ' + name)
    resp= session.get( quoteUrl )

    return resp.json()

def getExchangeDay( exchangeDay ):
    while exchangeDay.weekday() > 4 :
        exchangeDay -= datetime.timedelta(days=1)
    return exchangeDay

async def getStocks():
    # Select your transport with a defined url endpoint
    # TODO Use https - how ever gives ssl certificate error
    transport = HTTPXAsyncTransport(url="http://localhost:54676/graphql")

    # Create a GraphQL client using the defined transport
    # TODO authentication
    async with Client(
        transport=transport,
        fetch_schema_from_transport=True
    ) as session:
    # Provide a GraphQL query
        query = gql("""{stocks{stocks(take: 0, skip: 0 ) {name, isin, instrumentCode}}}""")
        gqlResponse = await session.execute(query)
        return gqlResponse['stocks']['stocks']

async def setStockData(stockDataMutation):
    # TODO Use https - how ever gives ssl certificate error
    transport = HTTPXAsyncTransport(url="http://localhost:54676/graphql")

    # Create a GraphQL client using the defined transport
    # TODO authentication
    async with Client(
        transport=transport,
        fetch_schema_from_transport=True
    ) as session:
    # Provide a GraphQL query        
        result = await session.execute(stockDataMutation)
        return result['stock']['isin']

def getTimeSeriesFactsFromJsonResponse(resp):
     return

def createStockDataMutationFromNasdaqNordicResponse(isin, timeseriename, resp):
    stringBuilder = []
    stringBuilder.Append("mutation{stock{adddata(stockDataInput:{isin:")
    stringBuilder.Append("\"")
    stringBuilder.Append(isin)
    stringBuilder.Append("\",")
    stringBuilder.Append("timeseriename:\"")
    stringBuilder.Append(timeseriename)
    stringBuilder.Append("\"},")
    stringBuilder.Append("timeSerieDataInput:")
    stringBuilder.Append("[")

			 { date:"2024-07-24T02:00:00.000Z", price:110.00, volume:1.00 }
    stringBuilder.Append("]){")
	stringBuilder.Append("isin")			
	stringBuilder.Append("}}}")

    return ''.join(stringBuilder)

async def main():
    nasdaqDateFormat = '%Y%m%d'
    fra = datetime.datetime.strptime( '20240801', nasdaqDateFormat)
    fra = getExchangeDay(fra).strftime(nasdaqDateFormat)
    til = getExchangeDay(datetime.date.today()).strftime(nasdaqDateFormat)

    stocks = await getStocks()
    for stock in stocks:
        resp = getStockPrizesFromNasdaqNordic( stock['instrumentCode'], stock['isin'], stock['name'], fra, til)
        stockDataMutation = createStockDataMutationFromNasdaqNordicResponse(resp)
        setResult = await setStockData(stockDataMutation)


asyncio.run(main())