# basic imports
import time
import json
import datetime

# http communication
import requests

# GraphQl client
import asyncio
import httpx
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
    session = requests.Session()
    session.headers = header

    resp = session.get(url)
    return resp.json()

def getStockPrizesAndVolumesFromXetra(isin, fromDate, toDate):
    # ISIN:   IE00B5BMR087
    # symbol: XETR:IE00B5BMR087
    # resolution: 1D
    # from:   1546300800
    # to:	    1745280000
    # countback:  329
    # https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3AIE00B5BMR087&resolution=1D&from=1546300800&to=1745280000&countback=329
    stringBuilder = []
    stringBuilder.append('https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3A')
    stringBuilder.append(isin)
    stringBuilder.append('&resolution=1D&from=')
    # TODO
    stringBuilder.append('TODO fromdate')
    stringBuilder.append('&to=')
    stringBuilder.append('TODO enddate')
    return ''.join(stringBuilder)

def getExchangeDay( exchangeDay ):
    while exchangeDay.weekday() > 4 :
        exchangeDay -= datetime.timedelta(days=1)
    return exchangeDay

def createCqlTransport(timeout):
    # Select your transport with a defined url endpoint
    # TODO Use https - how ever gives ssl certificate error
    # With proper certificate installed: propreturn HTTPXAsyncTransport(url="https://localhost:58879/graphql", timeout=timeout)
    return HTTPXAsyncTransport(url="http://localhost:54676/graphql", timeout=timeout)

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
    stringBuilder.append("take: 0, skip: 0 ) {name, isin, exchange, instrumentCode}}}")
    return ''.join(stringBuilder)

async def getStocks(isin='', exchange=''):
    # Create a GraphQL client using the defined transport
    # TODO authentication
    hxto = httpx.Timeout(300.0)
    transport = createCqlTransport(hxto)
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
    hxto = httpx.Timeout(300.0)
    transport = createCqlTransport(hxto)
    async with Client (
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

def convertXetraUnixDateToStringZuluDate( datetime, elisDateTimeFormat):
    # unix time "1736125200"
    return datetime.utcfromtimestamp(float(datetime)).strftime(elisDateTimeFormat)

def parseNasdaqData(x):
    result = x.replace(',','')
    if len(result) == 0:
        result = '0.00'
    return result

# Generator of date, prize tuple for Nasdaq
def getTimeSeriesFactsFromNasdaqJsonResponse(jsonResp, elisDateTimeFormat, nasdaqDateFormat):    
    data = jsonResp['data']
    chartData = data['chartData']
    cp = data['CP']
    for datePriceAndVolume in cp:
        z = datePriceAndVolume['z']
        dateTime = convertNasdaqDateToStringZuluDate(z['dateTime'], elisDateTimeFormat, nasdaqDateFormat)
        close = parseNasdaqData(z['close'])
        volume = parseNasdaqData(z['volume'])
        yield dateTime, close, volume

# Generator of date, prize tuple for Xetra
def getTimeSeriesFactsFromXetraJsonResponse(jsonResp, elisDateTimeFormat):
    # resp = '
    # {
    #   "s":"ok",
    #   "t":[1736125200,1736211600,1736298000,1736384400,1736470800,1736730000],
    #   "c":[89.77,89.71,89.88,90.07,89.54,88.23],
    #   "o":[88.5,89.91,89.75,89.31,90.03,88.65],
    #   "h":[89.77,90,90.54,90.14,90.19,88.86],
    #   "l":[88.33,89.31,89.47,89.31,89.11,87.75],
    #   "v":[4395680.47,1579085.22,3297222.87,515360.57,5000581.62,3485169.56]
    # }'
    status = jsonResp['s']
    if status == 'ok':
        timeAndDates = jsonResp['t']
        prizes = jsonResp['c']
        volumes = jsonResp['v']
        for i in range(len(timeAndDates)):
            unixtime = timeAndDates[i]
            prize = prizes[i]
            volume = volumes[i]
            dateTime = convertXetraUnixDateToStringZuluDate( unixtime, elisDateTimeFormat)
            # close = parseNasdaqData(prize)
            # volume = parseNasdaqData()
            yield dateTime, prize, volume
    else:
        return None

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
    for prizeAndVolume in getTimeSeriesFactsFromNasdaqJsonResponse(resp, elisDateTimeFormat, nasdaqDateFormat):
        if not first:
            stringBuilder.append(',')
        first = False
        appendDatePriceVolume(stringBuilder, prizeAndVolume)
    stringBuilder.append(']){')
    stringBuilder.append('isin')
    stringBuilder.append('}}}')

    return ''.join(stringBuilder)

def createStockDataMutationFromXetraResponse(isin, timeseriename, resp, elisDateTimeFormat):
    stringBuilder = []
    stringBuilder.append('mutation{stock{adddata(')
    appendStockDataInput(stringBuilder, isin, timeseriename)
    stringBuilder.append(',timeSerieDataInput:')
    stringBuilder.append("[")
    first = True
    for prizeAndVolume in getTimeSeriesFactsFromXetraJsonResponse(resp, elisDateTimeFormat):
        if not first:
            stringBuilder.append(',')
        first = False
        appendDatePriceVolume(stringBuilder, prizeAndVolume)
    stringBuilder.append(']){')
    stringBuilder.append('isin')
    stringBuilder.append('}}}')

    return ''.join(stringBuilder)

async def saveInCqlFile( name, data):
    fileName = name + '_Cql.txt'
    with open(fileName, "w") as txtfile:
        txtfile.write(data)

def getAndCreateStockDataMutationFromNasdaq(stock, elisDateTimeFormat, startDate, endDate):
    print('Gets from Nasdaq: ' + stock['name'] + ', ' + stock['isin'])
    resp = getStockPrizesAndVolumesFromNasdaq( stock['instrumentCode'], startDate, endDate)
    # 3. Store timeseries for stock in backend as GraphQL mutation
    stockDataMutation = createStockDataMutationFromNasdaqNordicResponse(stock['isin'], 'PriceAndVolume', resp, elisDateTimeFormat, nasdaqDateFormat)
    return stockDataMutation

def getAndCreateStockDataMutationFromXetra(stock, elisDateTimeFormat, startDate, endDate):
    print('Gets from Xetra: ' + stock['name'] + ', ' + stock['isin'])
    resp = getStockPrizesAndVolumesFromXetra( stock['isin'], startDate, endDate)
    # 3. Store timeseries for stock in backend as GraphQL mutation
    stockDataMutation = createStockDataMutationFromXetraResponse(stock['isin'], 'PriceAndVolume', resp, elisDateTimeFormat, nasdaqDateFormat)
    return stockDataMutation

async def getNasdaqStockData(beginDate, elisDateTimeFormat, nasdaqDateFormat, saveMutationInCqlFile=False):
    # 1. Get stock instrument code from backend
    stocks = await getStocks(exchange='XCSE') # get all nasdag nordic

    fromDate = datetime.datetime.strptime( beginDate, nasdaqDateFormat)
    startDate = getExchangeDay(fromDate).strftime(nasdaqDateFormat)
    endDate = getExchangeDay(datetime.date.today()).strftime(nasdaqDateFormat)

    # 2. Get and store stock data
    for stock in stocks:
        # Get timeseries from exchange as json response for the stock
        stockDataMutation = getAndCreateStockDataMutationFromNasdaq(stock, elisDateTimeFormat, startDate, endDate)
        if saveMutationInCqlFile:
            await saveInCqlFile( stock['isin'], stockDataMutation)
        result = await setStockData(stockDataMutation)
        # TODO Response from nasdag may be so slow that http transport times out for backend
        print('Stored ' + result)

async def getXetraStockData(beginDate, elisDateTimeFormat, saveMutationInCqlFile=False):
    # 1. Get stock instrument code from backend
    stocks = await getStocks(exchange='XETR') # get all Xetra

    # TODO
    fromDate = datetime.datetime.strptime( beginDate, nasdaqDateFormat)
    startDate = getExchangeDay(fromDate).strftime(nasdaqDateFormat)
    endDate = getExchangeDay(datetime.date.today()).strftime(nasdaqDateFormat)

    # 2. Get and store stock data
    for stock in stocks:
        # Get timeseries from exchange as json response for the stock
        stockDataMutation = getAndCreateStockDataMutationFromXetra(stock, elisDateTimeFormat, startDate, endDate)
        if saveMutationInCqlFile:
            await saveInCqlFile( stock['isin'], stockDataMutation)
        result = await setStockData(stockDataMutation)
        # TODO Response from nasdag may be so slow that http transport times out for backend
        print('Stored ' + result)


async def main(beginDate, elisDateTimeFormat, nasdaqDateFormat, saveMutationInCqlFile=False):

    print('get Nasdaq stock data...\n')
    await getNasdaqStockData(beginDate, elisDateTimeFormat, nasdaqDateFormat)
    # print('get Xetra stock data...\n')
    # await getXetraStockData(beginDate, elisDateTimeFormat)



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

def testcreateStockDataMutationFromXetraResponse(elisDateTimeFormat):
    # Isin: DE000A0H08J9
    # resolution: 1D
    # from:   1735919734
    # to:	    1736783734
    # countback:  2
    # https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3ADE000A0H08J9&resolution=1D&from=1735919734&to=1736783734&countback=2
    resp = json.loads("""{"s":"ok","t":[1736125200,1736211600,1736298000,1736384400,1736470800,1736730000],"c":[89.77,89.71,89.88,90.07,89.54,88.23],"o":[88.5,89.91,89.75,89.31,90.03,88.65],"h":[89.77,90,90.54,90.14,90.19,88.86],"l":[88.33,89.31,89.47,89.31,89.11,87.75],"v":[4395680.47,1579085.22,3297222.87,515360.57,5000581.62,3485169.56]}""")
    stockDataMutation = createStockDataMutationFromXetraResponse( 'DE000A0H08J9', 'PriceAndVolume', resp, elisDateTimeFormat, xetraDateFormat)
    print('')
    print(stockDataMutation)

elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
nasdaqDateFormat = '%Y-%m-%d'
beginDate = '2015-01-01'

#testcreateStockDataMutationFromNasdaqNordicResponse(elisDateTimeFormat, nasdaqDateFormat)
#testcreateStockDataMutationFromXetraResponse(elisDateTimeFormat)
asyncio.run(main(beginDate, elisDateTimeFormat, nasdaqDateFormat, False)) #  Set last argument to True to view resulting mutation