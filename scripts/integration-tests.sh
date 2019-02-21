set -ex

cd ../out/published
dotnet jaytwo.NuGetCheck.dll --packageId=xunit --minVersion=2.0.0 --maxVersion=2.0.1

echo "OK"