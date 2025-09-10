
from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class Process:
    def __init__(self):
        self.handler = None
        pass

    def Register(self, handler):
        self.handler = handler

    async def Execute(self, pipeElement):
        # Call GraphQL mutation handler. 
        # Set the mutation property in the pipe element to the result of the handler, when the handler
        # succeeded. Otherwise update status with the error.
        # The function returns the changed pipe element.
        if self.handler is not None:
            pipeElement = self.handler.handle(pipeElement)
        else:
            pipeElement.status.status = False
            pipeElement.status.message = 'Handler for GraphQL mutation processing not found.'

        return pipeElement