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

def getStockPrizesAndVolumesFromNasdaq(instrument, fromDate, toDate):
    # web API calls to Nasdaq:
    # https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en
    # https://api.nasdaq.com/api/nordic/instruments/CSE3456/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en

    url = 'https://api.nasdaq.com/api/nordic/instruments/'
    url += instrument 
    url += '/chart?assetClass=SHARES'
    url += '&fromDate=' + fromDate
    url += '&toDate=' + toDate
    url += '&lang=en'

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
    return resp.json()

def getExchangeDay( exchangeDay ):
    while exchangeDay.weekday() > 4 :
        exchangeDay -= datetime.timedelta(days=1)
    return exchangeDay

def createCqlTransport():
    # Select your transport with a defined url endpoint
    # TODO Use https - how ever gives ssl certificate error
    return HTTPXAsyncTransport(url="http://localhost:54676/graphql")

def buildCqlQueryString(isin='', exchange=''):
    # query = """{stocks{stocks( isin: "12323", take: 0, skip: 0 ) {name, isin, instrumentCode}}}""""
    # or
    # query = """{stocks{stocks( take: 0, skip: 0 ) {name, isin, instrumentCode}}}""" 
    stringBuilder = []
    stringBuilder.append("{stocks{stocks(")
    if isin:
        stringBuilder.append("isin:\"")
        stringBuilder.append(isin)
        stringBuilder.append("\",")
    if exchange:
        stringBuilder.append("exchange:\"")
        stringBuilder.append(exchange)
        stringBuilder.append("\",")
    stringBuilder.append("take: 0, skip: 0 ) {name, isin, instrumentCode}}}")
    return ''.join(stringBuilder)

async def getStocks(isin='', exchange=''):
    # Create a GraphQL client using the defined transport
    # TODO authentication
    transport = createCqlTransport()    
    async with Client(
        transport=transport,
        execute_timeout=300.0,
        fetch_schema_from_transport=True
    ) as session:
        # Provide a GraphQL query       
        queryString = buildCqlQueryString(isin, exchange)
        query = gql(queryString)
        gqlResponse = await session.execute(query)
        return gqlResponse['stocks']['stocks']

async def setStockData(stockDataMutation):
    # Create a GraphQL client using the defined transport
    # TODO authentication
    transport = createCqlTransport()
    async with Client(
        transport=transport,
        execute_timeout=300.0
    ) as session:
        # Provide a GraphQL mutation   
        mutation = gql(stockDataMutation)
        result = await session.execute(mutation)
        return result['stock']['adddata']['isin']

def convertUnixDateTimeToStringZuluDate(unixdate, elisDateTimeFormat):
     return datetime.datetime.fromtimestamp(int(unixdate/1000)).strftime(elisDateTimeFormat)

def convertNasdaqDateToStringZuluDate(date, elisDateTimeFormat, nasdaqDateFormat):
     return datetime.datetime.strptime(date,nasdaqDateFormat).strftime(elisDateTimeFormat)

# Generator of date, prize tuple
def getTimeSeriesFactsFromJsonResponse(jsonResp, elisDateTimeFormat, nasdaqDateFormat):    
    data = jsonResp['data']
    chartData = data['chartData']
    cp = data['CP']
    for datePrize in cp:
        z = datePrize['z']
        dateTime = convertNasdaqDateToStringZuluDate(z['dateTime'], elisDateTimeFormat, nasdaqDateFormat)
        close = z['close'].replace(',','')
        volume = z['volume'].replace(',','')
        yield dateTime, close, volume

def appendStockDataInput(stringBuilder, isin, timeseriename):
    stringBuilder.append("stockDataInput:{isin:")
    stringBuilder.append("\"")
    stringBuilder.append(isin)
    stringBuilder.append("\",")
    stringBuilder.append("timeseriename:\"")
    stringBuilder.append(timeseriename)
    stringBuilder.append("\"}")

def appendDatePriceVolume(stringBuilder, prizeAndVolume):
    stringBuilder.append("{date:\"")
    stringBuilder.append(prizeAndVolume[0])
    stringBuilder.append("\",")
    stringBuilder.append("price:")
    stringBuilder.append(prizeAndVolume[1])
    stringBuilder.append(",")
    stringBuilder.append("volume:")
    stringBuilder.append(prizeAndVolume[2])
    stringBuilder.append("}")

def createStockDataMutationFromNasdaqNordicResponse(isin, timeseriename, resp, elisDateTimeFormat, nasdaqDateFormat):
    stringBuilder = []
    stringBuilder.append('mutation{stock{adddata(')
    appendStockDataInput(stringBuilder, isin, timeseriename)
    stringBuilder.append(',timeSerieDataInput:')
    stringBuilder.append("[")
    first = True
    for prizeAndVolume in getTimeSeriesFactsFromJsonResponse(resp, elisDateTimeFormat, nasdaqDateFormat):
        if not first:
            stringBuilder.append(',')
        first = False
        appendDatePriceVolume(stringBuilder, prizeAndVolume)
    stringBuilder.append(']){')
    stringBuilder.append('isin')
    stringBuilder.append('}}}')

    return ''.join(stringBuilder)

async def main(beginDate, elisDateTimeFormat, nasdaqDateFormat):
    # TODO magic start date
    print('')

    # 1. Get stock instrument code from backend
    stocks = await getStocks(exchange='XCSE') # get all nasdag nordic

    fromDate = datetime.datetime.strptime( beginDate, nasdaqDateFormat)
    startDate = getExchangeDay(fromDate).strftime(nasdaqDateFormat)
    endDate = getExchangeDay(datetime.date.today()).strftime(nasdaqDateFormat)

    for stock in stocks:
        # 2. Get timeseries from Nasdaq Nordic as json response
        print('Gets from Nasdaq: ' + stock['name'] + ', ' + stock['isin'])
        resp = getStockPrizesAndVolumesFromNasdaq( stock['instrumentCode'], startDate, endDate)
        # 3. Store timeseries for stock in backend as GraphQL mutation
        stockDataMutation = createStockDataMutationFromNasdaqNordicResponse(stock['isin'], 'PriceAndVolume', resp, elisDateTimeFormat, nasdaqDateFormat)
        print(stockDataMutation)
        result = await setStockData(stockDataMutation)
        # TODO Response from nasdag may be so slow that http transport times out for backend
        print('Stored ' + result)


def testcreateStockDataMutationFromNasdaqNordicResponse(elisDateTimeFormat, nasdaqDateFormat):
    # Test with Acarix isin "SE0009268717", instrumentcode "SSE130710"
    # Response for web-api call: https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2025-01-06&toDate=2025-01-10&lang=en
    resp = json.loads("""{"data":{"chartData":{"orderbookId":"SSE130710","assetClass":"SHARES","isin":"SE0009268717","symbol":"ACARIX","company":"Acarix","timeAsOf":"2025-01-10","lastSalePrice":"SEK 0.2465","netChange":"-0.002","percentageChange":"-0.80%","deltaIndicator":"up","previousClose":"SEK 0.2485"},"CP":[{"z":{"dateTime":"2025-01-07","value":"0.2425","high":"0.243","low":"0.2375","open":"0.241","close":"0.2425","volume":"2,628,986"},"x":1736208000,"y":0.2425},{"z":{"dateTime":"2025-01-08","value":"0.2435","high":"0.25","low":"0.22","open":"0.25","close":"0.2435","volume":"6,548,534"},"x":1736294400,"y":0.2435},{"z":{"dateTime":"2025-01-09","value":"0.2595","high":"0.2595","low":"0.2405","open":"0.2435","close":"0.2595","volume":"1,543,435"},"x":1736380800,"y":0.2595},{"z":{"dateTime":"2025-01-10","value":"0.252","high":"0.262","low":"0.24","open":"0.26","close":"0.252","volume":"2,996,697"},"x":1736467200,"y":0.252}]},"messages":null,"status":{"timestamp":"2025-01-14T10:53:39+0100","rCode":200,"bCodeMessage":null,"developerMessage":""}}""")
    stockDataMutation = createStockDataMutationFromNasdaqNordicResponse( 'SE0009268717', 'PriceAndVolume', resp, elisDateTimeFormat, nasdaqDateFormat)
    print('')
    print(stockDataMutation)
    # Result
    # mutation {
    #   stock {
    #     adddata(
    #       stockDataInput: { isin: "SE0009268717", timeseriename: "PriceAndVolume" }
    #       timeSerieDataInput: [
    #         { date: "2025-01-07T00:00:00.000000Z", price: 0.2425, volume: 2628986 }
    #         { date: "2025-01-08T00:00:00.000000Z", price: 0.2435, volume: 6548534 }
    #         { date: "2025-01-09T00:00:00.000000Z", price: 0.2595, volume: 1543435 }
    #         { date: "2025-01-10T00:00:00.000000Z", price: 0.252, volume: 2996697 }
    #       ]
    #     ) {
    #       isin
    #     }
    #   }
    # }    
    # Should look like this:
    # mutation {
	#   stock {
 	# 	  adddata( 
	# 	   	stockDataInput: {isin: "isincode", timeseriename: "name"},
	# 	    timeSerieDataInput: [ { date:"date and time in zulu like: 2024-07-24T02:00:00.000Z", price: 110.00, volume: 1.00 } ] 
    #     ) {
    #       isin
    #     }
	#   }
    # }

elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
nasdaqDateFormat = '%Y-%m-%d'
beginDate = '2020-01-13'

testcreateStockDataMutationFromNasdaqNordicResponse(elisDateTimeFormat, nasdaqDateFormat)
#asyncio.run(main(beginDate, elisDateTimeFormat, nasdaqDateFormat))