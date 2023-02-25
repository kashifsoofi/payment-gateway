# Postman tests for Payments.Api

## Create Docker image
1. Execute following command to create docker image containing postman collections and environments  
`docker build -t webapi-service-postman .`

## Run postman collections
Run docker image to run postman collections with api service running on docker host (local.postman_environment.json)  
`docker run webapi-service-postman`  
To run collections with other environments  
`docker run webapi-service-postman --environment local.postman_environment.json`  
To run collections with environment variable not specified in environment file  
`docker run webapi-service-postman --environment local.postman_environment.json --env-var secure_environment_variable=my_secure_environment`  

## References & Resources
* https://github.com/postmanlabs/newman/tree/develop/docker
* https://learning.postman.com/docs/postman/scripts/test-examples/  
* https://stackoverflow.com/questions/43924363/newman-postmant-specify-a-single-environment-variable-via-command-line  
