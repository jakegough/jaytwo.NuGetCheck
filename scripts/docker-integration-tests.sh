set -x

rm -rf ../out/integrationTestResults
mkdir -p ../out

docker build -t $DOCKER_INTEGRATION_TEST_TAG ../ --target integration-test-results
docker create --name=$DOCKER_INTEGRATION_TEST_RESULTS_NAME $DOCKER_INTEGRATION_TEST_TAG
docker cp $DOCKER_INTEGRATION_TEST_RESULTS_NAME:/out/integrationTestResults ../out/integrationTestResults
docker rm $DOCKER_INTEGRATION_TEST_RESULTS_NAME
docker rmi $DOCKER_INTEGRATION_TEST_TAG

if [ -f ../out/integrationTestResults/.failed ]; then
  echo "'.failed' present, FAIL"
  exit 1
elif [ -f ../out/integrationTestResults/.passed ]; then
  echo "'.passed' present, OK"
  exit 0  
fi

echo "unknown test resutls, FAIL"
exit 2