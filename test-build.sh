#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}"

cd $cwd
find . -name bin -exec rm -rf {} +
find . -name obj -exec rm -rf {} +

cd $cwd/EasyLanguage
rm -rf Parser

cd $cwd/EasyLanguage
rm -rf Parser/ELang
mkdir -p Parser/ELang	
java -cp aparse-2.5.jar com.parse2.aparse.Parser \
  -language cs \
  -destdir Parser/ELang \
  -namespace Global.Parser.ELang \
  elang.abnf
cd Parser/ELang
ls *.cs | xargs -i sed -i "1,9d" {}
mv Parser.cs Parser.cs.txt

cd $cwd
dotnet test -p:Configuration=Release -p:Platform="Any CPU" EasyLanguage.sln

cd $cwd/EasyLanguage
#sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" EasyLanguage.csproj
rm -rf *.nupkg
dotnet pack -o . -p:Configuration=Release -p:Platform="Any CPU" EasyLanguage.csproj

exit 0

tag="EasyLanguage-v$version"
cd $cwd
git add .
git commit -m"$tag"
git tag -a "$tag" -m"$tag"
git push origin "$tag"
git push
git remote -v
