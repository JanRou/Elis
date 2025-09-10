import unittest
import datetime
import json

from context import Exchanges
from Exchanges.XetraClient import XetraClient

if __name__ == '__main__':
    unittest.main()

class TestXetraClient(unittest.TestCase):
    def __init__(self, *args, **kwargs):
        super(TestXetraClient, self).__init__(*args, **kwargs)
        self.elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'

    def testConvertXetraUnixDateToStringZuluDate(self):
        #Arrange
        expected = "2025-01-06T00:00:00.000000Z"
        unixtime = "1736125200"
        dut = XetraClient(self.elisDateTimeFormat)

        #Act
        result = dut.convertXetraUnixDateToStringZuluDate(unixtime, self.elisDateTimeFormat)

        #Assert
        self.assertEqual(expected, result)

    def testCreateXetraRequest(self):
        #Arrange
        # ISIN:   IE00B5BMR087
        # symbol: XETR:IE00B5BMR087
        # resolution: 1D
        # from:   1546300800
        # to:	    1745280000
        # countback:  329
        # https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3AIE00B5BMR087&resolution=1D&from=1546300800&to=1745280000&countback=329
        fromDate = datetime.datetime.strptime('2019-01-01T00:00:00.000000Z', self.elisDateTimeFormat)
        toDate = datetime.datetime.strptime('2025-04-22T00:00:00.000000Z', self.elisDateTimeFormat)
        isin = "IE00B5BMR087"
        dut = XetraClient(self.elisDateTimeFormat)
        expected = "https://api.boerse-frankfurt.de/v1/tradingview/history?symbol=XETR%3AIE00B5BMR087&resolution=1D&from=1546300800&to=1745280000"

        #Act
        result = dut.createXetraRequest(isin, fromDate, toDate)

        #Assert
        self.assertEqual(expected, result)

    def testGetPrizeAndVolumeGenerator(self):
        #Arrange
        jsonresp = json.loads("""
        {
          "s":"ok",
          "t":[1736125200,1736211600,1736298000,1736384400,1736470800,1736730000],
          "c":[89.77,89.71,89.88,90.07,89.54,88.23],
          "o":[88.5,89.91,89.75,89.31,90.03,88.65],
          "h":[89.77,90,90.54,90.14,90.19,88.86],
          "l":[88.33,89.31,89.47,89.31,89.11,87.75],
          "v":[4395680.47,1579085.22,3297222.87,515360.57,5000581.62,3485169.56]
        }""")
        expected = []
        expected.append(("2025-01-06T00:00:00.000000Z", 89.77, 4395680.47))
        expected.append(("2025-01-07T00:00:00.000000Z", 89.71, 1579085.22))
        expected.append(("2025-01-08T00:00:00.000000Z", 89.88, 3297222.87))
        expected.append(("2025-01-09T00:00:00.000000Z", 90.07,  515360.57))
        expected.append(("2025-01-10T00:00:00.000000Z", 89.54, 5000581.62))
        expected.append(("2025-01-13T00:00:00.000000Z", 88.23, 3485169.56))
        dut = XetraClient(self.elisDateTimeFormat)

        #Act + Assert
        ix = 0
        for prizeAndVolume in dut.getPrizeAndVolumeGenerator(jsonresp):
            self.assertLess(ix, len(expected), 'too many tuples returned by generator')
            self.assertEqual(expected[ix][0],prizeAndVolume[0])
            self.assertEqual(expected[ix][1],prizeAndVolume[1])
            self.assertEqual(expected[ix][2],prizeAndVolume[2])
            ix += 1
