# basic imports
import time
import datetime

from Core.Entities.Stock import Stock
from Core.Entities.Status import Status

class PipeElement:
    def __init__(self, fromDate, toDate, stock=None):
        self.fromDate = fromDate
        self.toDate = toDate
        self.stock = Stock()
        if stock is not None:
            # take from dict returned by cql query
            self.stock.name = stock['name']
            self.stock.isin = stock['isin']
            self.stock.exchange = stock['exchange']
            self.stock.instrumentcode = stock['instrumentCode']
        self.generator = ()
        self.mutation = ''
        self.status = Status()
