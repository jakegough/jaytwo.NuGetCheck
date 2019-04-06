TIMESTAMP?=$(shell date +'%Y%m%d%H%M%S')
DOCKER_TAG?=jaytwo_nugetcheck

default: clean build

clean: 
	find . -name bin | xargs --no-run-if-empty rm -vrf
	find . -name obj | xargs --no-run-if-empty rm -vrf
	rm -rf out

restore:
	dotnet restore . --verbosity minimal
  
build: restore
	dotnet build ./jaytwo.NuGetCheck.sln

run:
	dotnet run --project ./src/jaytwo.NuGetCheck -- --help

test: unit-test
  
unit-test: build
	rm -rf out/testResults
	dotnet test ./test/jaytwo.NuGetCheck.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.NuGetCheck.Tests.trx"

pack:
	rm -rf out/packed
	cd ./src/jaytwo.NuGetCheck; \
		dotnet pack -o ../../out/packed ${PACK_ARG}

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

publish:
	rm -rf out/published
	cd ./src/jaytwo.NuGetCheck; \
		dotnet publish -o ../../out/published

DOCKER_BUILDER_TAG?=${DOCKER_TAG}__builder
DOCKER_BUILDER_CONTAINER?=${DOCKER_BUILDER_TAG}
docker-builder:
	docker build -t ${DOCKER_BUILDER_TAG} . --target builder --pull

docker: docker-builder
	docker build -t ${DOCKER_TAG} . --pull
 
DOCKER_RUN_MAKE_TARGETS?=run
docker-run:
	docker run --name ${DOCKER_BUILDER_CONTAINER} ${DOCKER_BUILDER_TAG} make ${DOCKER_RUN_MAKE_TARGETS} || EXIT_CODE=$$? ; \
	docker cp ${DOCKER_BUILDER_CONTAINER}:build/out ./ || echo "Container not found: ${DOCKER_BUILDER_CONTAINER}"; \
	docker rm ${DOCKER_BUILDER_CONTAINER} || echo "Container not found: ${DOCKER_BUILDER_CONTAINER}"}; \
	exit $$EXIT_CODE

docker-unit-test-only: DOCKER_RUN_MAKE_TARGETS=unit-test
docker-unit-test-only: docker-run

docker-unit-test: docker-builder docker-unit-test-only

docker-pack-only: DOCKER_RUN_MAKE_TARGETS=pack
docker-pack-only: docker-run

docker-pack: docker-builder docker-pack-only

docker-pack-beta-only: DOCKER_RUN_MAKE_TARGETS=pack-beta
docker-pack-beta-only: docker-run

docker-pack-beta: docker-builder docker-pack-beta-only

docker-clean:
	docker rm ${DOCKER_BUILDER_CONTAINER} || echo "Container not found: ${DOCKER_BUILDER_CONTAINER}"
	docker rmi ${DOCKER_BUILDER_TAG} || echo "Image not found: ${DOCKER_BUILDER_TAG}"
	docker rmi ${DOCKER_TAG} || echo "Image not found: ${DOCKER_TAG}"
