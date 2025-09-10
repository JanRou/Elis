import datetime
import unittest
import json

from context import Exchanges
from Exchanges.NasdaqOmxClient import NasdaqOmxClient

if __name__ == '__main__':
    unittest.main()

class TestNasdaqOmxClient(unittest.TestCase):
    def __init__(self, *args, **kwargs):
        super(TestNasdaqOmxClient, self).__init__(*args, **kwargs)
        self.elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
        self.nasdaqDateFormat = '%Y-%m-%d'
    
    def testConvertNasdaqDateToStringZuluDate(self):
        #Arrange
        dut = NasdaqOmxClient(self.elisDateTimeFormat)
        expected = "2025-01-07T00:00:00.000000Z"
        #Act
        result = dut.convertNasdaqDateToStringZuluDate("2025-01-07", self.elisDateTimeFormat, self.nasdaqDateFormat)

        #Assert
        self.assertEqual(expected, result)

    def testCreateNasdaqOmxRequest(self):
        #Arrange
        # web API calls to Nasdaq:
        # https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en
        # https://api.nasdaq.com/api/nordic/instruments/CSE3456/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en        
        fromDate = datetime.datetime.strptime('2020-01-13T00:00:00.000000Z', self.elisDateTimeFormat)
        toDate = datetime.datetime.strptime('2025-01-13T00:00:00.000000Z', self.elisDateTimeFormat)
        instrument = 'SSE130710'
        expected = 'https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2020-01-13&toDate=2025-01-13&lang=en'
        dut = NasdaqOmxClient(self.elisDateTimeFormat)

        #Act
        result = dut.createNasdaqOmxRequest(instrument, fromDate, toDate)

        #Assert
        self.assertEqual(expected, result)

    def testGetPrizeAndVolumeGenerator(self):
        #Arrange
        # Test with Acarix isin "SE0009268717", instrumentcode "SSE130710"
        # Response for web-api call: https://api.nasdaq.com/api/nordic/instruments/SSE130710/chart?assetClass=SHARES&fromDate=2025-01-06&toDate=2025-01-10&lang=en
        resp = json.loads("""{"data":{"chartData":{"orderbookId":"SSE130710","assetClass":"SHARES","isin":"SE0009268717","symbol":"ACARIX","company":"Acarix","timeAsOf":"2025-01-10","lastSalePrice":"SEK 0.2465","netChange":"-0.002","percentageChange":"-0.80%","deltaIndicator":"up","previousClose":"SEK 0.2485"},"CP":[{"z":{"dateTime":"2025-01-07","value":"0.2425","high":"0.243","low":"0.2375","open":"0.241","close":"0.2425","volume":"2,628,986"},"x":1736208000,"y":0.2425},{"z":{"dateTime":"2025-01-08","value":"0.2435","high":"0.25","low":"0.22","open":"0.25","close":"0.2435","volume":"6,548,534"},"x":1736294400,"y":0.2435},{"z":{"dateTime":"2025-01-09","value":"0.2595","high":"0.2595","low":"0.2405","open":"0.2435","close":"0.2595","volume":"1,543,435"},"x":1736380800,"y":0.2595},{"z":{"dateTime":"2025-01-10","value":"0.252","high":"0.262","low":"0.24","open":"0.26","close":"0.252","volume":"2,996,697"},"x":1736467200,"y":0.252}]},"messages":null,"status":{"timestamp":"2025-01-14T10:53:39+0100","rCode":200,"bCodeMessage":null,"developerMessage":""}}""")
        dut = NasdaqOmxClient(self.elisDateTimeFormat) 
        # Expected result
        expected = []
        expected.append(("2025-01-07T00:00:00.000000Z", 0.2425, 2628986))
        expected.append(("2025-01-08T00:00:00.000000Z", 0.2435, 6548534))
        expected.append(("2025-01-09T00:00:00.000000Z", 0.2595, 1543435 ))
        expected.append(("2025-01-10T00:00:00.000000Z", 0.2520, 2996697 ))

        #Act + Assert
        ix = 0
        for prizeAndVolume in dut.getPrizeAndVolumeGenerator(resp):
            self.assertLess(ix, len(expected), 'too many tuples returned by generator')
            self.assertEqual(expected[ix][0],prizeAndVolume[0])
            self.assertEqual(expected[ix][1],prizeAndVolume[1])
            self.assertEqual(expected[ix][2],prizeAndVolume[2])
            ix += 1
