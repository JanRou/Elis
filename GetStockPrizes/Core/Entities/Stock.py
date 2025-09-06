class Stock:
    def __init__(self):
        self.isin = ''
        self.name = ''
        self.exchange = ''
        self.instrumentcode = ''

    def SetStock(self, isin, name, exchange, instrumentcode):
        self.isin = isin
        self.name = name
        self.exchange = exchange
        self.instrumentcode = instrumentcode
