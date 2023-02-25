param (
  $command
)

function Main() {  
  if ($command -eq "start") {
    Write-Host "Starting development environment"

    $localStackContainerId = docker ps -f "name=localstack" --format "{{.ID}}"
    if ($null -eq $localStackContainerId) {
      docker-compose -f docker-compose.localstack.yml up -d
      Start-Sleep -Seconds 10
      $localStackContainerId = docker ps -f "name=localstack" --format "{{.ID}}"
    }

    docker-compose -f docker-compose.dev-env.yml up -d
  }
  elseif ($command -eq "stop") {
    Write-Host "Stoping development environment"
    docker-compose -f docker-compose.dev-env.yml down -v --rmi local --remove-orphans

    $localStackContainerId = docker ps -f "name=nservicebus-with-localstack_localstack" --format "{{.ID}}"
    if ($null -ne $localStackContainerId) {
      docker-compose -f docker-compose.localstack.yml down -v --rmi local --remove-orphans
    }
  }  
}

Main