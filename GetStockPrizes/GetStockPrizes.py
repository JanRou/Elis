import requests
import time
import json
import datetime
from ftplib import FTP

def hentKurser(instrument, isin, navn, fra, til):
    ## konsolidatorUrlKurser='http://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE172620&name=Konsolidator&ISIN=DK0061113511'
    ## GAMLE konsolidatorUrl='http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=20200415&ToDate=20210416&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE172620'
    ## NYE konsolidatorUrl='https://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&FromDate=1986-01-01&ToDate=2024-06-25&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&Instrument=CSE172620'
    ## 
    # https://www.nasdaqomxnordic.com/aktier/microsite?Instrument=CSE172620&name=Konsolidator&ISIN=DK0061113511
    url  = 'http://www.nasdaqomxnordic.com/aktier/microsite?'
    url += 'Instrument=' + instrument
    url += '&name=' + navn
    url += '&ISIN=' + isin

    quoteUrl  = 'http://www.nasdaqomxnordic.com/webproxy/DataFeedProxy.aspx?SubSystem=History&Action=GetChartData&inst.an=id%2Cnm%2Cfnm%2Cisin%2Ctp%2Cchp%2Cycp&'
    quoteUrl += 'FromDate=' + fra
    quoteUrl += '&ToDate=' + til
    quoteUrl += '&json=true&showAdjusted=true&app=%2Faktier%2Fmicrosite-MicrositeChart-history&timezone=CET&DefaultDecimals=false&'
    quoteUrl += 'Instrument=' + instrument

    result = []
    header = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0'
            , 'Accept-Language': 'da,en-US;q=0.7,en;q=0.3'
            , 'Accept-Encoding': 'text/html, */*; q=0.01'
            , 'Connection': 'keep-alive'
            , 'Accept': 'application/json, text/javascript, */*; q=0.01'
            }
    session = requests.Session();
    session.headers = header;

    resp = session.get(url)
    time.sleep(5)

    session.headers.update({'X-Requested-With': 'XMLHttpRequest'})
    session.headers.update({'Content-Type':	'application/json; charset=UTF-8'})
    print('Henter kurser for ' + navn)
    resp= session.get( quoteUrl )

    kursData=resp.json()

    # gem i fil
    fileName = isin+'data.json'
    with open(fileName, 'w') as f:
        json.dump(kursData,f)
    f.close()
    print(fileName + ' er gemt')
    ## hent fra fil
    ##    with open('kursdata.json', 'r') as f:
    ##        kursData=json.loads(f.read())
        
    #if kursData['data']!=[]:
    #    data = kursData['data']
    #    chartData = data[0]['chartData']
    #    for kurs in chartData['cp']:
    #        udato=kurs[0]/1000
    #        pris=str(kurs[1])
    #        dato=datetime.datetime.fromtimestamp(int(udato)).strftime('%Y-%m-%d')
    #        result.append((dato,pris))
    #return result

def getBorsDag( borsDag ):
    while borsDag.weekday() > 4 :
        borsDag -= datetime.timedelta(days=1)
    return borsDag

# def upload(file):
#     # Upload kurser til site
#     # text fil ftp.storlines("STOR " + file, open(file))
#     ftp.storbinary("STOR " + file, open(file, "rb"), 1024)

konsolidator = {'navn':'Konsolidator', 'isin': 'DK0061113511', 'instrument': 'CSE172620'}
acarix = { 'navn': 'Acarix', 'isin': 'SE0009268717', 'instrument': 'SSE130710'}

nasdaqDateFormat = '%Y%m%d'
fra = datetime.datetime.strptime( '20201001', nasdaqDateFormat)
fra = getBorsDag(fra).strftime(nasdaqDateFormat)
til = getBorsDag(datetime.date.today()).strftime(nasdaqDateFormat)

##def hentKurser(instrument, isin, navn, fra, til):
hentKurser( konsolidator['instrument'], konsolidator['isin'], konsolidator['navn'], fra, til)
hentKurser( acarix['instrument'], acarix['isin'], acarix['navn'], fra, til)
print('Kurser hentet')

# ftp = FTP('files.000webhost.com')
# # folder /storage/ssd4/741/16722741
# ftp.connect()
# ftp.login('stockprizeautomation', 'lFsvhbzISPVh4Dd7saCH' )
# ftp.cwd('/public_html')    
# upload(konsolidator['isin']+'data.json')
# print(konsolidator['isin']+'data.json' + ' lagt op')
# upload(acarix['isin']+'data.json')
# print(acarix['isin']+'data.json' + ' lagt op')
# ftp.quit()
print('Slut')