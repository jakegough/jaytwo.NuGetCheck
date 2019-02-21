set -x

rm -rf out/published
mkdir -p ../out

docker build -t $DOCKER_PUBLISH_TAG ../ --target $DOCKER_PUBLISH_BUILD_TARGET
docker create --name=$DOCKER_PUBLISH_RESULTS_NAME $DOCKER_PUBLISH_TAG
docker cp $DOCKER_PUBLISH_RESULTS_NAME:/published ../out/published
docker rm $DOCKER_PUBLISH_RESULTS_NAME
docker rmi $DOCKER_PUBLISH_TAG
