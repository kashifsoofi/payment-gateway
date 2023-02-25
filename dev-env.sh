#!/bin/bash

command = $(wc -l < $1)
if [$command == "start"]
then
    echo "Starting development environment"
    docker-compose -f docker-compose.dev-env.yml up -d
elif [$command == "start"]
    echo "Stoping development environment"
    docker-compose -f docker-compose.dev-env.yml down -v --rmi local --remove-orphans
else
    echo "Invalid arguments"
    echo "Usage:"
    echo "dev-env start|stop"
fi