
from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class Process:
    def __init__(self):
        self.handler = None
        pass

    def Register(self, handler):
        self.handler = handler

    def Execute(self, pipeElement):
        # Call GraphQL mutation handler. 
        # Set the mutation property in the pipe element to the result of the handler, when the handler
        # succeeded. Otherwise update status with the error.
        # The function returns the changed pipe element.
        mutation = ''
        result = (mutation, Status())
        if self.handler is not None:
            result = self.handler.handle(pipeElement)
        else:
            result[1].status = False
            result[1].message = 'Handler for GraphQL mutation processing not found.'

        pipeElement.status = result[1]
        if result[1].status:
            pipeElement.mutation = result[0]
        return pipeElement