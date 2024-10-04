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
    #print('Get prizes for ' + name)
    resp= session.get( quoteUrl )

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

def convertUnixDateTimeToStringZuluDate(unixdate):
     elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
     return datetime.datetime.fromtimestamp(int(unixdate/1000)).strftime(elisDateTimeFormat)

# Generator of date, prize tuple
def getTimeSeriesFactsFromJsonResponse(jsonResp):    
    data = jsonResp['data']
    chartData = data[0]['chartData']
    for datePrize in chartData['cp']:
        date = convertUnixDateTimeToStringZuluDate(datePrize[0])
        prize = str(datePrize[1])
        yield date, prize

def appendStockDataInput(stringBuilder, isin, timeseriename):
    stringBuilder.append("stockDataInput:{isin:")
    stringBuilder.append("\"")
    stringBuilder.append(isin)
    stringBuilder.append("\",")
    stringBuilder.append("timeseriename:\"")
    stringBuilder.append(timeseriename)
    stringBuilder.append("\"},")

def appendDatePrize(stringBuilder, datePrize):
    stringBuilder.append("{date:\"")
    stringBuilder.append(datePrize[0])
    stringBuilder.append("\",")
    stringBuilder.append("price:")
    stringBuilder.append(datePrize[1])
    stringBuilder.append(",")
    stringBuilder.append("volume:")
    stringBuilder.append("0.00}")

def createStockDataMutationFromNasdaqNordicResponse(isin, timeseriename, resp):
    stringBuilder = []
    stringBuilder.append("mutation{stock{adddata(")
    appendStockDataInput(stringBuilder, isin, timeseriename)
    stringBuilder.append(",timeSerieDataInput:")
    stringBuilder.append("[")
    first = True
    for datePrize in getTimeSeriesFactsFromJsonResponse(resp):
        if not first:
            stringBuilder.append(",")
        else:
            first = False
        appendDatePrize(stringBuilder, datePrize)
    stringBuilder.append("]){")
    stringBuilder.append("isin")
    stringBuilder.append("}}}")

    return ''.join(stringBuilder)

async def main():
    # For debug
    # jsonResp = json.loads("""{"@status": "1", "@ts": "1727529812289", "data": [{"instData": {"@id": "SSE130710", "@nm": "ACARIX", "@fnm": "Acarix", "@isin": "SE0009268717", "@tp": "S", "@chp": "0.0", "@ycp": "0.307"}, "chartData": {"cp": [[1722470400000, 0.49], [1722556800000, 0.449], [1722816000000, 0.42], [1722902400000, 0.424], [1722988800000, 0.4385], [1723075200000, 0.439], [1723161600000, 0.406], [1723420800000, 0.405], [1723507200000, 0.3925], [1723593600000, 0.36], [1723680000000, 0.36], [1723766400000, 0.3665], [1724025600000, 0.325], [1724112000000, 0.3445], [1724198400000, 0.33], [1724284800000, 0.3195], [1724371200000, 0.308], [1724630400000, 0.274], [1724716800000, 0.2765], [1724803200000, 0.289], [1724889600000, 0.282], [1724976000000, 0.28], [1725235200000, 0.356], [1725321600000, 0.3215], [1725408000000, 0.316], [1725494400000, 0.3105], [1725580800000, 0.339], [1725840000000, 0.347], [1725926400000, 0.339], [1726012800000, 0.304], [1726099200000, 0.3215], [1726185600000, 0.314], [1726444800000, 0.314], [1726531200000, 0.3255], [1726617600000, 0.335], [1726704000000, 0.3335], [1726790400000, 0.33], [1727049600000, 0.304], [1727136000000, 0.2965], [1727222400000, 0.301], [1727308800000, 0.307], [1727395200000, 0.307]]}}]}""")

    print('')

    # 1. Get stock instrument code from backend
    stocks = await getStocks(exchange='XCSE') # get all nasdag nordic

    nasdaqDateFormat = '%Y%m%d'
    fraDato = datetime.datetime.strptime( '20180101', nasdaqDateFormat)
    start = getExchangeDay(fraDato).strftime(nasdaqDateFormat)
    end = getExchangeDay(datetime.date.today()).strftime(nasdaqDateFormat)

    for stock in stocks:
        # 2. Get timeseries from Nasdaq Nordic as json response
        print('Henter fra Nasdaq: ' + stock['name'] + ', ' + stock['isin'])
        resp = getStockPrizesFromNasdaqNordic( stock['instrumentCode'], stock['isin'], stock['name'], start, end)
        # 3. Store timeseries for stock in backend as GraphQL mutation
        stockDataMutation = createStockDataMutationFromNasdaqNordicResponse(stock['isin'], 'DateAndPrize', resp)
        print(stockDataMutation)
        result = await setStockData(stockDataMutation)
        # TODO Response from nasdag may be so slow that http transport times out for backend
        print('Gemt i db: ' + result)


asyncio.run(main())