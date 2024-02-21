SELECT 
	  st."Name"
	, st."Isin"
	, cu."Short" AS "Currency"
	, ex."ExchangeUrl"
FROM public."Stocks" st
JOIN public."Exchanges" ex ON ex."Id" = st."ExchangeId"
JOIN public."Currencies" cu ON cu."Id" = st."CurrencyId"
;