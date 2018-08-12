docker build -t $DOCKER_UNIT_TEST_TAG ../ --target unit-test-results
docker create --name=$DOCKER_UNIT_TEST_RESULTS_NAME $DOCKER_UNIT_TEST_TAG
docker cp $DOCKER_UNIT_TEST_RESULTS_NAME:/testResults ../
docker rm $DOCKER_UNIT_TEST_RESULTS_NAME
docker rmi $DOCKER_UNIT_TEST_TAG

if [ -f ../testResults/.failed ]; then
  echo "'.failed' present, failing script"
  exit 1
fi