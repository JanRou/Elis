
class CqlStocksQueryBuilder:

    def buildCqlStocksQuery(self, isin='', exchange=''):
        # query = """{stocks{stocks( isin: "12323", exchange: "exchange", take: 0, skip: 0 ) {name, isin, exchange, instrumentCode}}}""""
        # or
        # query = """{stocks{stocks( isin: "12323", take: 0, skip: 0 ) {name, isin, exchange, instrumentCode}}}"""
        # or
        # query = """{stocks{stocks( take: 0, skip: 0 ) {name, isin, exchange, instrumentCode}}}""" 
        # Response example:
        # {
        #   "data": {
        #     "stocks": {
        #       "stocks": [
        #         {
        #           "name": "Acarix",
        #           "isin": "SE0009268717",
        #           "exchange": "XCSE",
        #           "instrumentCode": "SSE130710"
        #         },
        #         {
        #           "name": "ALK-Abelló B A/S",
        #           "isin": "DK0061802139",
        #           "exchange": "XCSE",
        #           "instrumentCode": "CSE32679"
        #         }
        #        ]
        #      }
        #    }
        # }
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
        stringBuilder.append("take:0,skip:0){name,isin,exchange,instrumentCode}}}")
        return ''.join(stringBuilder)