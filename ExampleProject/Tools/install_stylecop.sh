#!/bin/bash

brew install mono

curl https://dist.nuget.org/win-x86-commandline/v3.3.0/nuget.exe > /usr/local/bin/nuget.exe

cd $HOME/code

git clone https://github.com/nearlyfreeapps/StyleCop.Baboon.git

cd $HOME/code/StyleCop.Baboon

mono --runtime=v4.0 /usr/local/bin/nuget.exe restore

xbuild "StyleCop.Baboon.sln"

mkdir -p /usr/local/opt/StyleCop.Baboon

cp $HOME/code/StyleCop.Baboon/StyleCop.Baboon/bin/Debug/* /usr/local/opt/StyleCop.Baboon/

printf '%s\n%s' '#!/bin/bash' 'exec $(which mono) /usr/local/opt/StyleCop.Baboon/StyleCop.Baboon.exe "$@"' > /usr/local/bin/stylecop

chmod a+x /usr/local/bin/stylecop

cd $HOME/code

git clone https://github.com/uken/Unity.StyleCop.Rules.git

cd $HOME/code/Unity.StyleCop.Rules

xbuild "Unity.StyleCop.Rules.sln"

cp $HOME/code/Unity.StyleCop.Rules/Unity.StyleCop.Rules/bin/Debug/Unity.StyleCop.CSharp.Rules.dll /usr/local/opt/StyleCop.Baboon/Unity.StyleCop.CSharp.Rules.dll
