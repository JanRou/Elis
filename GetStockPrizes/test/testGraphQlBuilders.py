import unittest
import datetime
import json

import context
from Gateways.CqlQueryBuilder import CqlStocksQueryBuilder
from Gateways.CqlStockMutationBuilder import CqlStockMutationBuilder
from Core.Entities.PipeElement import PipeElement
from Exchanges.XetraClient import XetraClient

if __name__ == '__main__':
    unittest.main()

class TestGraphQlBuilders(unittest.TestCase):
    def __init__(self, *args, **kwargs):
        super(TestGraphQlBuilders, self).__init__(*args, **kwargs)
        self.elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'

    def testBuildCqlQueryString(self):
        #Arrange
        url  = "http://localhost:54676/graphql"
        isin = "12323"
        exchange = "exchange"
        expected = """{stocks{stocks(isin:"12323",exchange:"exchange",take:0,skip:0){name,isin,exchange,instrumentCode}}}"""
        dut = CqlStocksQueryBuilder()
        
        #Act
        result = dut.buildCqlStocksQuery(isin, exchange)

        #Assert
        self.assertEqual(result, expected)

    def testBuildStockMutationBuilder(self):
        # Arrange
        fromDate = datetime.datetime.strptime( '2020-01-01', '%Y-%m-%d')
        toDate = datetime.datetime.strptime( '2025-01-02', '%Y-%m-%d')
        pipeElement = PipeElement(fromDate, toDate)
        pipeElement.stock.isin = 'IE00B5BMR087'
        pipeElement.generator = self.createGenerator()
        dut = CqlStockMutationBuilder()
        
        # Act
        result = dut.handle(pipeElement)

        # Assert
        self.assertTrue(result.status.status)
        self.assertIsNotNone(result.mutation)
        self.assertNotEqual (result.mutation, '')


    def createGenerator(self):
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
        xetraClient = XetraClient(self.elisDateTimeFormat)
        return xetraClient.getPrizeAndVolumeGenerator(jsonresp) 