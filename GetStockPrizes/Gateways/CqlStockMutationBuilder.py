from Core.Entities.PipeElement import PipeElement
from Core.Entities.Status import Status

class CqlStockMutationBuilder:

    # inteface method for process, returns (mutation, status)
    def handle(self, pipeElement):
        pipeElement.mutation = self.createStockDataMutation(pipeElement.stock.isin, 'PriceAndVolume', pipeElement.generator)
        # TODO evaluate mutation and set status
        pipeElement.status = Status()
        return pipeElement

    def createStockDataMutation( self, isin, timeseriename, prizeAndVolumeGenerator):
        stringBuilder = []
        stringBuilder.append('mutation{stock{adddata(')
        self.appendStockDataInput(stringBuilder, isin, timeseriename)
        stringBuilder.append(',timeSerieDataInput:')
        stringBuilder.append("[")
        first = True
        for prizeAndVolume in prizeAndVolumeGenerator:
            if not first:
                stringBuilder.append(',')
            first = False
            self.appendDatePriceVolume(stringBuilder, prizeAndVolume)
        stringBuilder.append(']){')
        stringBuilder.append('isin')
        stringBuilder.append('}}}')

        return ''.join(stringBuilder)


    def appendStockDataInput(self, stringBuilder, isin, timeseriename):
        stringBuilder.append("stockDataInput:{isin:")
        stringBuilder.append("\"")
        stringBuilder.append(isin)
        stringBuilder.append("\",")
        stringBuilder.append("timeseriename:\"")
        stringBuilder.append(timeseriename)
        stringBuilder.append("\"}")

    def appendDatePriceVolume(self, stringBuilder, prizeAndVolume):
        stringBuilder.append("{date:\"")
        stringBuilder.append(prizeAndVolume[0])
        stringBuilder.append("\",")
        stringBuilder.append("price:")
        stringBuilder.append(str(prizeAndVolume[1]))
        stringBuilder.append(",")
        stringBuilder.append("volume:")
        stringBuilder.append(str(prizeAndVolume[2]))
        stringBuilder.append("}")
