# basic imports
import time
import datetime

from Core.Entities.Stock import Stock
from Core.Entities.Status import Status

class PipeElement:
    def __init__(self, fromDate, toDate):
        self.fromDate = fromDate
        self.toDate = toDate
        self.stock = Stock()
        self.generator = ()
        self.mutation = ''
        self.status = Status()
