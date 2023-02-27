param (
  $command
)

function Main() {  
  if ($command -eq "start") {
    Write-Host "Starting payment gateway"

    docker-compose -f docker-compose.yml up -d
  }
  elseif ($command -eq "stop") {
    Write-Host "Stoping payment gateway"
    docker-compose -f docker-compose.yml down -v --rmi local --remove-orphans
  }  
}

Main