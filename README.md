# Payments
This is `dotnet new` payments for a ASP.NET Core Web API service.

## Run with Visual Studio
1. Clone repository locally.
2. Open Powershell command at the root of the Dirctory and issue following to start MySql database in a docker container.  
`.\dev-env.ps1 start`
3. Open `Payments.sln` in Visual Studio.
4. Set `Payments.Api` and `Payments.Host` as startup projects.
5. Debug would start web browser with Api Url and Host would start a command window.
6. Afater finishing issue following command to stop and close database container.

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
MySql container is used for storage, to use a different storage layer, main changes would be in sql, Query classes and aggregate repository.

## Publish nuget packages from docker
1. build docker image  
`docker build --build-arg Version=0.1.0 -t payments.publisher . -f Dockerfile.Publisher`
2. run docker image to publish nuget packages  
`docker run -t -v ~/myfeed:/myfeed payments.publisher:latest --source /myfeed`  
Or with key  
`docker run -t -v ~/myfeed:/myfeed payments.publisher:latest --source /myfeed -k <apiKey>`


## References & Resources
* [ref](http://example.com)
