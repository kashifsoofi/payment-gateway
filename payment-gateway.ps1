param (
  $command
)

function Main() {  
  if ($command -eq "start") {
    Write-Host "Starting payment gateway"

    # docker-compose is not building application images, build images manually
    docker build -f src/AcquiringBank.Api/Dockerfile -t acquiringbank.api .
    docker build -f src/Payments.Api/Dockerfile -t payments.api .
    docker build -f src/Payments.Host/Dockerfile -t payments.host .

    docker-compose -f docker-compose.yml up -d
  }
  elseif ($command -eq "stop") {
    Write-Host "Stoping payment gateway"
    docker-compose -f docker-compose.yml down -v --rmi local --remove-orphans

    docker image rm -f acquiringbank.api
    docker image rm -f payments.api
    docker image rm -f payments.host

    docker rmi -f $(docker images -f "dangling=true" -q)
  }
}

Main