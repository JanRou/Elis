SELECT 
	  st."Name"
	, st."Isin"
	, st."InstrumentCode"
	, cu."Code" AS "Currency"
	, ex."Url"
FROM public."Stocks" st
JOIN public."Exchanges" ex ON ex."Id" = st."ExchangeId"
JOIN public."Currencies" cu ON cu."Id" = st."CurrencyId"
;