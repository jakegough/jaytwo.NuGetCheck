set -x

rm -rf ../out/testResults
mkdir -p ../out

docker build -t $DOCKER_UNIT_TEST_TAG ../ --target unit-test-results
docker create --name=$DOCKER_UNIT_TEST_RESULTS_NAME $DOCKER_UNIT_TEST_TAG
docker cp $DOCKER_UNIT_TEST_RESULTS_NAME:/out/testResults ../out/testResults
docker rm $DOCKER_UNIT_TEST_RESULTS_NAME
docker rmi $DOCKER_UNIT_TEST_TAG

if [ -f ../out/testResults/.failed ]; then
  echo "'.failed' present, FAIL"
  exit 1
elif [ -f ../out/testResults/.passed ]; then
  echo "'.passed' present, OK"
  exit 0  
fi

echo "unknown test resutls, FAIL"
exit 2