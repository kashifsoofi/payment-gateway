Feature: GetPayment
A merchant should be able to get a previously created payment.

@tag1
Scenario: Get Payment By Id
	Given a payment exists for a merchant
	When client gets a payment
	Then payment should be returned
