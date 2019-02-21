set -x

rm -rf out/packed
mkdir -p ../out

docker build -t $DOCKER_PACK_TAG ../ --target $DOCKER_PACK_BUILD_TARGET
docker create --name=$DOCKER_PACK_RESULTS_NAME $DOCKER_PACK_TAG
docker cp $DOCKER_PACK_RESULTS_NAME:/out/packed ../out/packed
docker rm $DOCKER_PACK_RESULTS_NAME
docker rmi $DOCKER_PACK_TAG
