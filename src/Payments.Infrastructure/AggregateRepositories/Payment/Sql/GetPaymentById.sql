SELECT
    Id,
	MerchantId,
    CreatedOn,
    UpdatedOn,
	CardHolderName,
	CardNumber,
	ExpiryMonth,
	ExpiryYear,
	Amount,
	CurrencyCode,
	Reference,
	Status
FROM Payment
WHERE Id = @Id