TIMESTAMP=$(shell date +'%Y%m%d%H%M%S')

default: build

clean: 
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	find . -name publish | xargs rm -vrf
	find . -name project.lock.json | xargs rm -vrf
	find . -name testResults | xargs rm -vrf
	find . -name out | xargs rm -vrf

restore:
	dotnet restore . --verbosity minimal
  
build: restore
	dotnet build ./src/jaytwo.NuGetCheck

run: build
	dotnet run  --project ./src/jaytwo.NuGetCheck

unit-test: build
	dotnet test ./test/jaytwo.NuGetCheck.Tests \
		--results-directory ../../testResults \
		--logger "trx;LogFileName=jaytwo.NuGetCheck.Tests.trx"
  
integration-test: build
	cd ./scripts; \
		chmod +x ./integration-tests.sh; \
		./integration-tests.sh

test: unit-test integration-test

publish: clean build
	cd ./src/jaytwo.NuGetCheck; \
		dotnet publish -o ../../out ${PACK_ARG}

publish-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
publish-beta: publish

DOCKER_TAG_PREFIX?=jaytwonugetcheck
DOCKER_TAG?=${DOCKER_TAG_PREFIX}${DOCKER_TAG_SUFFIX}
docker-build:
	docker build -t ${DOCKER_TAG} . --target builder
  
docker-test: docker-build
	docker run --rm ${DOCKER_TAG} make test

DOCKER_UNIT_TEST_TAG?=${DOCKER_TAG}__unit_test_results
DOCKER_UNIT_TEST_RESULTS_NAME?=${DOCKER_TAG}__unit_test_results_${TIMESTAMP}
docker-unit-test: docker-build
	export DOCKER_UNIT_TEST_TAG=${DOCKER_UNIT_TEST_TAG}; \
	export DOCKER_UNIT_TEST_RESULTS_NAME=${DOCKER_UNIT_TEST_RESULTS_NAME}; \
	cd ./scripts; \
		chmod +x ./docker-unit-tests.sh; \
		./docker-unit-tests.sh

docker-integration-test: docker-build
	docker run --rm ${DOCKER_TAG} make integration-test

docker-run: docker-build
	docker build -t ${DOCKER_TAG} . --target builder
  
DOCKER_PACK_MAKE_TARGETS?=pack
docker-pack: docker-build
	docker run --rm ${DOCKER_TAG} make ${DOCKER_PACK_MAKE_TARGETS}

docker-pack-beta: DOCKER_PACK_MAKE_TARGETS=pack-beta
docker-pack-beta: docker-pack

docker-publish: docker-build
	docker run --rm ${DOCKER_TAG} make publish
  
docker-cleanup:
	docker rmi ${DOCKER_TAG} || echo "docker tag ${DOCKER_TAG} not found"
	docker rm ${DOCKER_UNIT_TEST_RESULTS_NAME} || echo "docker tag ${DOCKER_TAG} not found"
	docker rmi ${DOCKER_UNIT_TEST_TAG} || echo "docker tag ${DOCKER_TAG} not found"
