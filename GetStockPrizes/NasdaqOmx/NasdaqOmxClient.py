# basic imports
import time
import json
import datetime

# http communication
import requests

class NasdaqOmxClient:
    def __init__(self, elisDateTimeFormat, nasdaqDateFormat):
        self.elisDateTimeFormat = elisDateTimeFormat
        self.nasdaqDateFormat = nasdaqDateFormat        
        self.jsonResp = None

    def getStockPrizesAndVolumesFromNasdaq(self, instrument, fromDate, toDate):
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
        self.jsonResp = resp.json()

    # TODO used?
    def convertUnixDateTimeToStringZuluDate(self, unixdate, elisDateTimeFormat):
        return datetime.datetime.fromtimestamp(int(unixdate/1000)).strftime(elisDateTimeFormat)

    def convertNasdaqDateToStringZuluDate(self, date, elisDateTimeFormat, nasdaqDateFormat):
        return datetime.datetime.strptime(date,nasdaqDateFormat).strftime(elisDateTimeFormat)

    def parseNasdaqData(self, x):
        result = x.replace(',','')
        if len(result) == 0:
            result = '0.00'
        return result

    # Generator of date, prize tuple for Nasdaq
    def getTimeSeriesFactsFromNasdaqJsonResponse(self, jsonResp):
        # Handle no jsonResp
        if self.jsonResp != None:
            data = jsonResp['data']
            chartData = data['chartData']
            cp = data['CP']
            for datePriceAndVolume in cp:
                z = datePriceAndVolume['z']
                dateTime = self.convertNasdaqDateToStringZuluDate(z['dateTime'], self.elisDateTimeFormat, self.nasdaqDateFormat)
                close = self.parseNasdaqData(z['close'])
                volume = self.parseNasdaqData(z['volume'])
                yield dateTime, close, volume

