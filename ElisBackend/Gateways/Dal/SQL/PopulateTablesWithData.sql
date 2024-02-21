DELETE FROM public."Exchanges";
ALTER TABLE public."Exchanges" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Exchanges"( "Name", "Country","ExchangeUrl") VALUES('XCSE', 'Danmark','https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","ExchangeUrl") VALUES('XHEL', 'Finland', 'https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","ExchangeUrl") VALUES('XSTO', 'Sverige', 'https://www.nasdaqomxnordic.com/' );
INSERT INTO public."Exchanges"( "Name", "Country","ExchangeUrl") VALUES('XETR', 'Tyskland', 'https://www.xetra.com/');
SELECT *
FROM public."Exchanges";

DELETE FROM public."Currencies";
ALTER TABLE public."Currencies" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Currencies"(	"Name", "Short") VALUES ( 'Danske kroner', 'DKK');
INSERT INTO public."Currencies"(	"Name", "Short") VALUES ( 'Svenske kroner', 'SEK');
INSERT INTO public."Currencies"(	"Name", "Short") VALUES ( 'Norske kroner', 'NOK');
INSERT INTO public."Currencies"(	"Name", "Short") VALUES ( 'Euro', 'EUR');
SELECT *
FROM public."Currencies";

DELETE FROM public."Stocks";
ALTER TABLE public."Stocks" ALTER COLUMN "Id" RESTART WITH 1;
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'Novo Nordisk B','DK0062498333', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'Rockwool A/S ser. B','DK0010219153', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'DSV A/S','DK0060079531', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'ALK-Abelló B A/S','DK0061802139', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'Schouw & Co. A/S','DK0010253921', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'GN Store Nord A/S','DK0010272632', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'Novozymes B A/S','DK0060336014', 1, 1);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'iShares Core S&P 500 UCITS ETF USD (Acc)','IE00B5BMR087', 4, 4);
INSERT INTO public."Stocks" ("Name", "Isin", "CurrencyId", "ExchangeId") VALUES ( 'iShares STOXX Europe 600 Technology UCITS ETF (DE)','DE000A0H08Q4', 4, 4);
SELECT *
FROM  public."Stocks";
	