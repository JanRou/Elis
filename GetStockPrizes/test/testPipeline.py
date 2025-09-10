import unittest
from unittest import IsolatedAsyncioTestCase
from unittest.mock import AsyncMock, MagicMock
import datetime
import asyncio

from Core.Entities.PipeElement import PipeElement
from Core.Entities.Stock import Stock
from Core.Entities.Status import Status

from Core.Application.Acquire import Acquire
from Core.Application.Process import Process
from Core.Application.Handover import Handover

from Exchanges.NasdaqOmxClient import NasdaqOmxClient
from Gateways.CqlStockMutationBuilder import CqlStockMutationBuilder
from Gateways.ElisGraphQlClient import ElisCqlClient

if __name__ == '__main__':
    unittest.main()

class TestPipeline(IsolatedAsyncioTestCase):
    def __init__(self, *args, **kwargs):
        super(TestPipeline, self).__init__(*args, **kwargs)
        self.elisDateTimeFormat = '%Y-%m-%dT%H:%M:%S.%fZ'
        
    async def testAcquire(self):
        #Arrange
        message = 'All good'
        exchangeClientMock = self.createExchangeClientMock(message)
        dut = Acquire()
        exchange = 'exchange'
        dut.Register(exchangeClientMock, exchange)
        pipeElement = self.createPipeElement(exchange)

        #Act
        result = await dut.Execute(pipeElement)

        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.generator)
        self.assertIsNotNone(result.status)
        self.assertTrue(result.status.status)
        self.assertEqual(message, result.status.message)

    async def testAcquireHandlerNotFound(self):
        #Arrange
        exchangeClientMock = self.createExchangeClientMock('All good')
        dut = Acquire()
        exchange = 'exchange'
        dut.Register(exchangeClientMock, exchange)
        pipeElement = self.createPipeElement('bad-exchange')

        #Act
        result = await dut.Execute(pipeElement)

        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.status)
        self.assertFalse(result.status.status)

    async def testProcess(self):
        #Arrange
        cqlStockMutationBuilderMock = CqlStockMutationBuilder()
        pipeElementWithMutation = self.createPipeElement('exchange')
        pipeElementWithMutation.mutation = 'Mutation OK'
        cqlStockMutationBuilderMock.handle = MagicMock(return_value=pipeElementWithMutation)
        dut = Process()
        dut.Register(cqlStockMutationBuilderMock)
        pipeElement = self.createPipeElement('exchange')

        #Act
        result = await dut.Execute(pipeElement)

        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.status)
        self.assertTrue(result.status.status)
        self.assertEqual(result.mutation, pipeElementWithMutation.mutation)

    async def testProcessHandlerNotFound(self):
        #Arrange
        dut = Process()
        pipeElement = self.createPipeElement('exchange')

        #Act
        result = await dut.Execute(pipeElement)

        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.status)
        self.assertFalse(result.status.status)

    async def testHandover(self):
        #Arrange
        elisCqlClientMock = ElisCqlClient("https://someexchange.com")
        pipeElementWithOk = self.createPipeElement('exchange')
        elisCqlClientMock.handle = AsyncMock(return_value=pipeElementWithOk)
        dut = Handover()
        dut.Register(elisCqlClientMock)
        pipeElement = self.createPipeElement('exchange')

        #Act
        result = await dut.Execute(pipeElement)
        
        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.status)
        self.assertTrue(result.status.status)

    async def testHandoverHandlerNotFound(self):
        #Arrange
        dut = Handover()
        pipeElement = self.createPipeElement('exchange')

        #Act
        result = await dut.Execute(pipeElement)
        
        #Assert
        self.assertIsNotNone(result)
        self.assertIsNotNone(result.status)
        self.assertFalse(result.status.status)

    def createPipeElement(self, exchange, isin='test'):
        fromDate = datetime.datetime.strptime( '2020-01-01', '%Y-%m-%d')
        toDate = datetime.datetime.strptime( '2025-01-02', '%Y-%m-%d')
        result = PipeElement(fromDate, toDate)
        result.stock.exchange = isin
        result.stock.exchange = exchange
        return result 

    def createExchangeClientMock(self, message):
        exchangeClientMock = NasdaqOmxClient(self.elisDateTimeFormat)
        generator = (x*x for x in range(1, 6))
        status = Status()
        status.status = True
        status.message = message
        result = (generator, status)        
        exchangeClientMock.handle = MagicMock(return_value=result)
        return exchangeClientMock
