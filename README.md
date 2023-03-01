# Payment Gateway
This is a very simple payments gateway that allows Merchants to take payments given Customer Card Info and amount.

Only `GBP` currency is supported in current solution.

## Requirements
The product requirements for this initial phase are the following:
1. A merchant should be able to process a payment through the payment gateway and receive either
a successful or unsuccessful response.
2. A merchant should be able to retrieve the details of a previously made payment. The next section
will discuss each of these in more detail.

### Assumptions
1. Client would be responsible for generating PaymentId, this would also service as Idempotency Key for our service.
2. All requests must include a `MerchantId` as header, its a lightweight authentication, I did not get time to add API Key/JWT key authentication, so instead I assumed this authentication would be performed by the API Gateway.
3. Create Payment request is asynchronous, I assumed there would be a lot of requests so the process of creating request is offloaded to the Host project that can be scaled independently.
4. AcquiringBank API call is synchronous and it returns success/failure when returning response.


### Extra mile
1. Implemented asynchronous request processing, allowing to independtly scale Api and Host (background message processor). Api further can be divided in a reader/writer api project.
2. Implemented a client api project and added a sample that would requires the services to be started as described in Containerisation or it can be run from VS after updating the url in appsettings.json file of `PaymentGateway.ApiClient.Sample`
3. Acceptance test project using SpecFlow, idea is to run those after deployment it is missing cleanup so safer to run in environment where test data does not make any difference.
4. Logs are being forwarded to [Seq](https://datalust.co/seq) and can be viewed at http://localhost:8080, both during development and when running all applications as containers.
5. Prometheus is setup to collect basic metrics published by prometheus-net from Payments.Api and Payments.Host
6. Grafana is setup with a dashboard with prometheus-net metrics.

## Run with Visual Studio
1. Clone repository locally.
2. Open Powershell command at the root of the Dirctory and issue following to start LocalStack and MySql database in docker containers.  
`.\dev-env.ps1 start`
3. Open `Payments.sln` in Visual Studio.
4. Set `Payments.Api`, `Payments.Host` and `AcquiringBank.Api` as startup projects.
5. Debug would start web browser with Api Url and Host would start a command window.
6. Afater finishing issue following command to stop and close database and LocalStack containers.

## Project Structure
### Contracts
This project contains commands, events and common classes to perform operations expected by this service.

### Domain
This project contains aggregate that this service is responsible for. Aggregate implements data and business checks and for each change creates an event.

### Infrastructure
This project contains code that is not part of the domain and mainly ineracts with exernal systems e.g. database.

### Api
This is WebApi project that exposes the REST endpoints to perform CRUD operations on Payment data. API directly reads data from database with help of Queries and for updates (create/update) sends commands to Host (Messag Processor).  

### Host
This is a commandline application. This is message processor of the system. It implements Handlers that listen for commands sent by API and perform those operations on Payment aggregate.

### Database
MySql container is used for storage, to use a different storage layer, main changes would be in sql, Query classes and aggregate repository. Migrations are maintained as a set of sql files and can be executed using the migrations container, this is part of the docker-compose.dev-env.yml file.

## NServiceBus/LocalStack
NServiceBus is used for message based communication with SQS as the backend for NServiceBus. Commands/Events are sent to SQS and are received by listening on those SQS queues. SQS queues are setup as part of the dev-env using `localstack-setup` container, this is part of the docker-compose.dev-env.yml file.

## AcquiringBank.Api
This is a WebApi project, it is exposing an endpoint to create a payment in AcquiringBank, this is a very simple simulator to Accept/Reject payments from the payment gateway.  
Assumption is that payments endpoint works synchronously and would return the result when the api call finishes.

Following card numbers would result in a success response.
- 4242424242424242
- 4273149019799094
- 4539467987109256
- 4024007181869214
- 4916301720257093

Following card numbers would result in an invalid expiry response.
- 4659959652550685
- 4446900535698356

Following card numbers would result in an insufficient funds response.
- 4140253846048187
- 4556447238607884

## Running Integration Tests
Integrations tests depend on the database and LocalStack running, so dev-env must be setup by running dev-env.ps1 script and ensuring both database and localstack containers are running and db-migration and localstack-setup containers have exited.  

## Running Acceptance Tests
Acceptance tests should be run against a deployed version of the service. To simulate that all pre-requsites and services must be started using `.\payment-gateway.ps1 start` (please see Containerisation section below for details) command before running `Payments.Api.Tests.Acceptance` project.  

# Containerisation
All the services are containerised and can be run using the `docker-compose.yml` file.  
However there was a bug with docker-compose not building .net application images on windows, so I have prepared a Powershell script to build the images before running all services using docker-compose. Please use following command to run all the services as containers.
```
.\payment-gateway.ps1 start
```
This would spin up containers for all services. I have implemented a delayed start for `Payments.Api` and `Payments.Hosts` projects however start up might fail if LocalStack/MySql container takes long time to start. In that case just running those servies from Docker Desktop UI would be fine.  
Once all the services are up `Payments Api` will be available at `http://localhost:5000/index.html`. Sample request to create a payment through swagger UI as follows  
```
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cardHolderName": "Chcuk Norris",
  "cardNumber": "4242424242424242",
  "expiryMonth": 12,
  "expiryYear": 2023,
  "cvv": "123",
  "amount": 100.25,
  "currencyCode": "GBP",
  "reference": "REF 001"
}
```
Please use any guid as merchant id, you would need this to get the payment details later.

Once finished, please use the following command to shutdown all the services, it would also delete the containers and cleanup any dangling containers from the system.
```
.\payment-gateway.ps1 stop
```

# PaymentGatewayApiClient
I have also added a PaymentGatewayApiClient project that can be packaged as a nuget and can be used to connect with the PaymentGateway. I have used a generic name as it can be used to add clients for other services e.g. Authentication.  
I have also added a Sample application using the PaymentGatewayApiClient. This requires all services are running using `payment-gateway.ps1` powershell script.

## References & Resources
[prometheus-net Dashboard](https://github.com/AChehre/prometheus-net-dashboard)
[Adding distributed tracing instrumentation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs)
[MySqlConnector Tracing](https://mysqlconnector.net/tutorials/tracing/)
[OpenTelemetry](https://docs.particular.net/nservicebus/operations/opentelemetry)
[How to setup OpenTelemetry instrumentation in ASP.NET core](https://dev.to/jmourtada/how-to-setup-opentelemetry-instrumentation-in-aspnet-core-23p5)