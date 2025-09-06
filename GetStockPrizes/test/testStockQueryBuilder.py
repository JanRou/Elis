import unittest

from Gateways.CqlQueryBuilder import CqlStocksQueryBuilder

if __name__ == '__main__':
    unittest.main()

class TestStockQueryBuilder(unittest.TestCase):

    def testbuildCqlQueryString(self):
        # Should build query
        
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
