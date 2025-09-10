import asyncio
import httpx

from gql import Client, gql
from gql.transport.httpx import HTTPXAsyncTransport
from Core.Entities import PipeElement
from Core.Entities import Status

class ElisCqlClient:
    def __init__(self, url):
          self.url = url # "http://localhost:54676/graphql"

    # inteface method for handover, returns (status)
    def handle(self, pipeElement):
        result = self.setStockData(pipeElement.mutation)
        pipeElement.status.status = result == pipeElement.stock.isin      
        if pipeElement.status.status:
            pipeElement.status.message = ''
        else:
            pipeElement.status.message = 'Handover mutation failed'
        return pipeElement

    def createCqlTransport(self, timeout):
        # Select your transport with a defined url endpoint
        # TODO Use https - how ever gives ssl certificate error
        # With proper certificate installed: propreturn HTTPXAsyncTransport(url="https://localhost:58879/graphql", timeout=timeout)
        return HTTPXAsyncTransport(url=self.url, timeout=timeout)

    async def getStocks(self, query):
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
            queryString = query
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

