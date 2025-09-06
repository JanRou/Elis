
class Status:
    def __init__(self):
        self.status = True
        self.message = ''
    
    def SetStatus(self, status, message):
        self.status = status
        self.message = message

    