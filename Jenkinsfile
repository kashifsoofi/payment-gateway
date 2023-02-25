import groovy.json.JsonOutput

def CURRENT_VERSION = ""

node {
    def gitInfo

    def commonParameters = [
        projectName: 'payments'
    ]

    stage('Build & Test') {
        echo 'Building and testing'
        gitInfo = checkout scm
        setVersionNumber()

        try {
            runTests()
        }
        catch (runTestsException) {
            cleanupTests();
            throw runTestsException
        }
        finally {
            publishTestResults()
        }

        // buildAndPushImages(commonParameters)
    }

    stage('Publish packages') {
        echo 'Publishing packages'
        // publishPackages()
    }
}

def setVersionNumber() {
	def majorMinor = "1.0"
	if (fileExists('version.txt')) {
		majorMinor = readFile('version.txt').trim()
	}

	CURRENT_VERSION = "${majorMinor}.${BUILD_NUMBER}"
}

def runTests() {
    echo "Running tests"
    docker.image('tiangolo/docker-with-compose').inside {
        sh "docker-compose -f docker-compose.testrunner.yml run testrunner"
    }
}

def cleanupTests() {
    echo "Cleaning up tests"
    docker.image('tiangolo/docker-with-compose').inside {
        sh "docker-compose -f docker-compose.testrunner.yml down --rmi local -v --remove-orphans"
    }
}

def publishTestResults() {
    // mstest testResultsFile:"./testresults/*.trx", keepLongStdio: true
}

def buildAndPushImages(parameters) {
    def registry = "my-registry:50001"
    echo "Building ${parameters.projectName}"

    def PaymentsCoreApi = docker.build("${registry}/payments-core-api:${CURRENT_VERSION}", "-f ./src/Payments.WebApi/Dockerfile .")
    def PaymentsCoreEndpoint = docker.build("${registry}/payments-core-endpoint:${CURRENT_VERSION}", "-f ./src/Payments.MessageProcessor/Dockerfile .")

    echo "Push image to registry"
    PaymentsCoreApi.push()
    PaymentsCoreEndpoint.push()
}

def publishPackages() {
    def registry = "my-registry:50001"
    def imageName = "${registry}/payments-package-publisher:${CURRENT_VERSION}"
    def packagePublisher = docker.build(imageName, "--build-arg Version=${CURRENT_VERSION} -f ./Dockerfile.Publisher .")
	try {
		packagePublisher.run("--rm -v c:/Dev/packages:/packages", "--source /packages")
	}
	finally {
		echo "Removing publisher image"
		docker.node {
			docker.script.sh "docker rmi ${imageName} -f"
		}
	}
}