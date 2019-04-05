TIMESTAMP?=$(shell date +'%Y%m%d%H%M%S')
DOCKER_TAG?=jaytwo_nugetcheck

default: clean build

clean: 
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	find . -name publish | xargs rm -vrf
	find . -name project.lock.json | xargs rm -vrf
	find . -name out | xargs rm -vrf

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
	docker build -t ${DOCKER_BUILDER_TAG} . --target builder

docker: docker-builder
	docker build -t ${DOCKER_TAG} .
 
DOCKER_RUN_MAKE_TARGETS?=pack
docker-run:
	# TODO: this will not fail the make target since it's just semicolon-ing all the way to the end
	docker run --name ${DOCKER_BUILDER_CONTAINER} ${DOCKER_BUILDER_TAG} make ${DOCKER_RUN_MAKE_TARGETS}; \
	docker cp ${DOCKER_BUILDER_CONTAINER}:build/out ./; \
	docker rm ${DOCKER_BUILDER_CONTAINER}

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
