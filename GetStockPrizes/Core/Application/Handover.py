
from Core.Entities.PipeElement import PipeElement

class Handover:
    def __init__(self):
        self.handler = None

    def Register(self, handler):
        self.handler = handler

    def Execute(self, pipeElement):
        if self.handler is not None:
            pipeElement = self.handler.handle(pipeElement)
        else:
            pipeElement.status.status = False
            pipeElement.status.message = 'Handler for handover mutation not found.'

        return pipeElement        
