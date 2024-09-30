-- Create exchanges
DELETE FROM public."Exchanges";
ALTER TABLE public."Exchanges" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Exchanges"( "Name", "Country","Url") VALUES('XCSE', 'Danmark','https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","Url") VALUES('XHEL', 'Finland', 'https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","Url") VALUES('XSTO', 'Sverige', 'https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","Url") VALUES('XETR', 'Tyskland', 'https://www.xetra.com/');
SELECT *
FROM public."Exchanges";

--Create currencies
DELETE FROM public."Currencies";
ALTER TABLE public."Currencies" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Currencies"(	"Name", "Code") VALUES ( 'Danske kroner', 'DKK');
INSERT INTO public."Currencies"(	"Name", "Code") VALUES ( 'Svenske kroner', 'SEK');
INSERT INTO public."Currencies"(	"Name", "Code") VALUES ( 'Norske kroner', 'NOK');
INSERT INTO public."Currencies"(	"Name", "Code") VALUES ( 'Euro', 'EUR');
SELECT *
FROM public."Currencies";

-- Create stocks
DELETE FROM public."Stocks";
ALTER TABLE public."Stocks" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Novo Nordisk B','DK0062498333','CSE1158', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Rockwool A/S ser. B','DK0010219153','CSE3456', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'DSV A/S','DK0060079531','CSE3415', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'ALK-Abelló B A/S','DK0061802139','CSE32679', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Schouw & Co. A/S','DK0010253921','CSE3259', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'GN Store Nord A/S','DK0010272632','CSE3205', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Novonesis B A/S','DK0060336014','CSE11273', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Konsolidator A/S','DK0061113511','CSE172620', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'Acarix','SE0009268717','SSE130710', 2, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'iShares Core S&P 500 UCITS ETF USD (Acc)','IE00B5BMR087','', 4, 4);
INSERT INTO public."Stocks" ("Name", "Isin", "InstrumentCode", "CurrencyId", "ExchangeId") VALUES ( 'iShares STOXX Europe 600 Technology UCITS ETF (DE)','DE000A0H08Q4','', 4, 4);
SELECT *
FROM  public."Stocks";
	