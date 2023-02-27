Feature: CreatePayment
A merchant should be able to create payment using payment gateway

@tag1
Scenario: Create payment
	Given a valid merchant id
	And cardNumber '4242424242424242' and reference 'REF001'
	When the client creates a payment
	Then create payment result should be success
