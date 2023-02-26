SELECT
    Id,
	MerchantId,
	CardHolderName,
	CardNumber,
	ExpiryMonth,
	ExpiryYear,
	Amount,
	CurrencyCode,
	Reference,
	Status,
	CreatedOn,
	UpdatedOn
FROM Payment
WHERE Id = @Id
AND MerchantId = @MerchantId