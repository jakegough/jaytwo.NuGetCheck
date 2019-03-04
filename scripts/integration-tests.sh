set -ex

cd ../out/published
dotnet jaytwo.NuGetCheck.dll xunit -gt 2.0.0 --same-major

echo "OK"