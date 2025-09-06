from Core.Entities import PipeElement
from Core.Entities import Status

class CqlStockMutationBuilder:

    # inteface method for process, returns (mutation, status)
    def handle(self, pipeElement):
        mutation = ''
        result = (mutation, Status())
        result.mutation = self.createStockDataMutation(pipeElement.stock.isin, 'PriceAndVolume', pipeElement.generator)
        # TODO evaluate mutation and set status
        result.status = Status()
        return result

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

    def appendDatePriceVolume(stringBuilder, prizeAndVolume):
        stringBuilder.append("{date:\"")
        stringBuilder.append(prizeAndVolume[0])
        stringBuilder.append("\",")
        stringBuilder.append("price:")
        stringBuilder.append(prizeAndVolume[1])
        stringBuilder.append(",")
        stringBuilder.append("volume:")
        stringBuilder.append(prizeAndVolume[2])
        stringBuilder.append("}")
