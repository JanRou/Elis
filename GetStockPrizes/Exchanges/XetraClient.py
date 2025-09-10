# basic imports
import time
import json
import datetime

# http communication
import requests

from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class XetraClient:
    def __init__(self, elisDateTimeFormat):
        self.elisDateTimeFormat = elisDateTimeFormat # ??? Elis date time format?
        self.xetraDateFormat = '' # TODO find the format
        self.jsonResp = None

    # inteface method for acquire stock data, returns (generator, status)
    def handle(self, pipeElement):
        jsonData = self.getStockPrizesAndVolumes( pipeElement.stock.isin, pipeElement.stock.instrumentcode, pipeElement.fromDate, pipeElement.toDate)
        # TODO check jsonData or return status from getStockPrizesAndVolumes
        status = Status()
        result = (self.getPrizeAndVolumeGenerator(jsonData), status)
        return result
    
    def convertXetraUnixDateToStringZuluDate(self, dt, elisDateTimeFormat):
        # unix time "1736125200"
        return datetime.date.fromtimestamp(float(dt)).strftime(elisDateTimeFormat) 

    def convertDateTimeToUnixDateTime(self, dateTime):
        return dateTime.timestamp()

    def createXetraRequest(self, isin, fromDate, toDate):
        # ISIN:   IE00B5BMR087
        # symbol: XETR:IE00B5BMR087
        # resolution: 1D
        # from:   1546300800
        # to:	    1745280000
        # countback:  329 (optional)
        # https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3AIE00B5BMR087&resolution=1D&from=1546300800&to=1745280000
        stringBuilder = []
        stringBuilder.append('https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3A')
        stringBuilder.append(isin)
        stringBuilder.append('&resolution=1D&from=')
        stringBuilder.append(str(int(fromDate.replace(tzinfo=datetime.timezone.utc).timestamp())))
        stringBuilder.append('&to=')
        stringBuilder.append(str(int(toDate.replace(tzinfo=datetime.timezone.utc).timestamp())))
        return ''.join(stringBuilder)

    def getStockPrizesAndVolumes(self, isin, instrument, fromDate, toDate):
        url = self.createXetraRequest(isin, fromDate, toDate)
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

    # Generator of date, prize tuple for Xetra
    def getPrizeAndVolumeGenerator(self, jsonResp):
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
                dateTime = self.convertXetraUnixDateToStringZuluDate( unixtime, self.elisDateTimeFormat)
                yield dateTime, prize, volume
        else:
            return None
