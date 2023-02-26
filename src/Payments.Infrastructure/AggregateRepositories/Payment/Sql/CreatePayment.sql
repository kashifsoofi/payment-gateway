INSERT INTO Payment (
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
)
VALUES (
	@Id,
	@MerchantId,
	@CardHolderName,
	@CardNumber,
	@ExpiryMonth,
	@ExpiryYear,
	@Amount,
	@CurrencyCode,
	@Reference,
	@Status,
	@CreatedOn,
	@UpdatedOn
)