TIMESTAMP=$(shell date +'%Y%m%d%H%M%S')

default: build

clean: 
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	find . -name publish | xargs rm -vrf
	find . -name project.lock.json | xargs rm -vrf
	find . -name out | xargs rm -vrf

restore:
	dotnet restore . --verbosity minimal
  
build: restore
	dotnet build ./src/jaytwo.NuGetCheck

run: build
	dotnet run  --project ./src/jaytwo.NuGetCheck

unit-test: build
	dotnet test ./test/jaytwo.NuGetCheck.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.NuGetCheck.Tests.trx"
  
integration-test: build
	cd ./scripts; \
		chmod +x ./integration-tests.sh; \
		./integration-tests.sh

test: unit-test integration-test

pack: clean build
	cd ./src/jaytwo.NuGetCheck; \
		dotnet pack -o ../../out/packed ${PACK_ARG}

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

publish:
	cd ./src/jaytwo.NuGetCheck; \
		dotnet publish -o ../../out/published ${PACK_ARG}

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
  
DOCKER_PACK_BUILD_TARGET?=packer-results
DOCKER_PACK_TAG?=${DOCKER_TAG}__pack
DOCKER_PACK_RESULTS_NAME?=${DOCKER_TAG}__pack_${TIMESTAMP}
docker-pack: docker-build
	export DOCKER_PACK_BUILD_TARGET=${DOCKER_PACK_BUILD_TARGET}; \
	export DOCKER_PACK_TAG=${DOCKER_PACK_TAG}; \
	export DOCKER_PACK_RESULTS_NAME=${DOCKER_PACK_RESULTS_NAME}; \
	cd ./scripts; \
		chmod +x ./docker-pack.sh; \
		./docker-pack.sh

docker-pack-beta: DOCKER_PACK_BUILD_TARGET=packer-beta-results
docker-pack-beta: docker-pack

docker-publish: docker-build
	docker run --rm ${DOCKER_TAG} make publish
  
docker-cleanup:
	docker rmi ${DOCKER_TAG} || echo "docker tag ${DOCKER_TAG} not found"
	docker rm ${DOCKER_UNIT_TEST_RESULTS_NAME} || echo "container ${DOCKER_UNIT_TEST_RESULTS_NAME} not found"
	docker rmi ${DOCKER_UNIT_TEST_TAG} || echo "docker tag ${DOCKER_UNIT_TEST_TAG} not found"
	docker rm ${DOCKER_PACK_RESULTS_NAME} || echo "container ${DOCKER_PACK_RESULTS_NAME} not found"
	docker rmi ${DOCKER_PACK_TAG} || echo "docker tag ${DOCKER_PACK_TAG} not found"
