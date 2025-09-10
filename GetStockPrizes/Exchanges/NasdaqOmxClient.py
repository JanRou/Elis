# basic imports
import time
import json
import datetime

# http communication
import requests

from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class NasdaqOmxClient():
    def __init__(self, elisDateTimeFormat):
        self.elisDateTimeFormat = elisDateTimeFormat
        self.nasdaqDateFormat = '%Y-%m-%d'

    # interface method to acquire stock data, returns (generator, status)
    def handle(self, pipeElement):
        jsonData = self.getStockPrizesAndVolumes( pipeElement.stock.isin, pipeElement.stock.instrumentcode, pipeElement.fromDate, pipeElement.toDate)
        # TODO check jsonData or return status from getStockPrizesAndVolumes
        status = Status()
        result = (self.getPrizeAndVolumeGenerator(jsonData), status)
        return result

    def createNasdaqOmxRequest(self, instrument, fromDate, toDate):
        # from and to dates are of type Python datetime, so they have to be converted to a string
        # web API calls to Nasdaq:
        # https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en
        # https://api.nasdaq.com/api/nordic/instruments/CSE3456/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en

        url = 'https://api.nasdaq.com/api/nordic/instruments/'
        url += instrument 
        url += '/chart?assetClass=SHARES'
        url += '&fromDate=' + fromDate.strftime(self.nasdaqDateFormat)
        url += '&toDate=' + toDate.strftime(self.nasdaqDateFormat)
        url += '&lang=en'
        return url

    def getStockPrizesAndVolumes(self, isin, instrument, fromDate, toDate):
        url = self.createNasdaqOmxRequest(instrument, fromDate, toDate)
        header = {
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0'
                , 'Accept-Language': 'da,en-US;q=0.7,en;q=0.3'
                , 'Accept-Encoding': 'text/html, */*; q=0.01'
                , 'Connection': 'keep-alive'
                , 'Accept': 'application/json, text/javascript, */*; q=0.01'
                }
        # TODO error handling
        session = requests.Session()
        session.headers = header

        resp = session.get(url)
        return resp.json()

    def convertNasdaqDateToStringZuluDate(self, date, elisDateTimeFormat, nasdaqDateFormat):
        return datetime.datetime.strptime(date,nasdaqDateFormat).strftime(elisDateTimeFormat)

    def parseNasdaqData(self, x):
        result = x.replace(',','')
        if len(result) == 0:
            result = '0.00'
        return float(result)

    # Generator of date, prize tuple for Nasdaq
    def getPrizeAndVolumeGenerator(self, jsonResp):
        # Handle no jsonResp and convert to common elis datetime format
        if jsonResp != None:
            data = jsonResp['data']
            chartData = data['chartData']
            cp = data['CP']
            for datePriceAndVolume in cp:
                z = datePriceAndVolume['z']
                dateTime = self.convertNasdaqDateToStringZuluDate(z['dateTime'], self.elisDateTimeFormat, self.nasdaqDateFormat)
                close = self.parseNasdaqData(z['close'])
                volume = self.parseNasdaqData(z['volume'])
                yield dateTime, close, volume
