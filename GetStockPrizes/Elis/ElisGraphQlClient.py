import asyncio
import httpx

from gql import Client, gql
from gql.transport.httpx import HTTPXAsyncTransport

class ElisCqlClient:
    def __init__(self, url):
          self.url = url # "http://localhost:54676/graphql"

    def createCqlTransport(self, timeout):
        # Select your transport with a defined url endpoint
        # TODO Use https - how ever gives ssl certificate error
        # With proper certificate installed: propreturn HTTPXAsyncTransport(url="https://localhost:58879/graphql", timeout=timeout)
        return HTTPXAsyncTransport(url=self.url, timeout=timeout)

    def buildCqlQueryString(self, isin='', exchange=''):
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

    async def getStocks(self, isin='', exchange=''):
        # Create a GraphQL client using the defined transport
        # TODO authentication
        hxto = httpx.Timeout(300.0)
        transport = self.createCqlTransport(hxto)
        async with Client(
            transport=transport,
            execute_timeout=300.0,
            fetch_schema_from_transport=True
        ) as session:
            # Provide a GraphQL query       
            queryString = self.buildCqlQueryString( isin, exchange)
            query = gql(queryString)
            gqlResponse = await session.execute(query)
            return gqlResponse['stocks']['stocks']

    async def setStockData(self, stockDataMutation):
        # Create a GraphQL client using the defined transport
        # TODO authentication
        hxto = httpx.Timeout(300.0)
        transport = self.createCqlTransport(hxto)
        async with Client (
            transport=transport,
            execute_timeout=300.0
        ) as session:
            # Provide a GraphQL mutation   
            mutation = gql(stockDataMutation)
            result = await session.execute(mutation)
            return result['stock']['adddata']['isin']

    def appendStockDataInput(self, stringBuilder, isin, timeseriename):
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

    def createStockDataMutation( self, isin, timeseriename, prizeAndVolumeGenerator):
        stringBuilder = []
        stringBuilder.append('mutation{stock{adddata(')
        self.appendStockDataInput(stringBuilder, isin, timeseriename)
        stringBuilder.append(',timeSerieDataInput:')
        stringBuilder.append("[")
        first = True
        for prizeAndVolume in prizeAndVolumeGenerator:
            if not first:
                stringBuilder.append(',')
            first = False
            self.appendDatePriceVolume(stringBuilder, prizeAndVolume)
        stringBuilder.append(']){')
        stringBuilder.append('isin')
        stringBuilder.append('}}}')

        return ''.join(stringBuilder)
