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

test: build
	dotnet test ./test/jaytwo.NuGetCheck.Tests
  
publish: clean build
	cd ./src/jaytwo.NuGetCheck & \
    dotnet publish -o ../../out ${PACK_ARG}

publish-beta: PACK_ARG=--version-suffix beta-$(shell date +'%Y%m%d%H%M%S')
publish-beta: publish

DOCKER_TAG?=jaytwonugetcheck
docker-build: clean
	docker build -t ${DOCKER_TAG} . --target builder
  
docker-test: docker-build
	docker run --rm ${DOCKER_TAG} make test

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
	docker rmi ${DOCKER_TAG}
