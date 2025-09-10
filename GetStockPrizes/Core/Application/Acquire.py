
from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class Acquire:
    def __init__(self):
        self.handlers = dict()

    def Register(self, handler, key):
        self.handlers[key] = handler

    async def Execute(self, pipeElement):
        # Look up handler registered to get data from exchange for the stock in the dictionary of handlers.
        # The stock exchange is key.
        # Call the handler, when found. Otherwise set error status false and a message that stays handler
        # not found.
        # Set the generator property in the pipe element to the result of the handler, when the handler
        # succeeded. Otherwise update status with the error.
        # The function returns the changed pipe element.
        result = (None, Status())
        if pipeElement.stock.exchange in self.handlers:
            result = self.handlers[pipeElement.stock.exchange].handle(pipeElement)
        else:
            result[1].status = False
            result[1].message = 'Handler for exchange ' + pipeElement.stock.exchange + ' not found.'

        pipeElement.status = result[1]
        if result[1].status:
            pipeElement.generator = result[0]
        return pipeElement
