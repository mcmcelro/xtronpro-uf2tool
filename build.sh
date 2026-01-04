dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r win-x64
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r win-arm64
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r linux-x64
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r linux-arm
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r linux-arm64
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r osx-x64
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=Link -p:EnableCompressionInSingleFile=true -p:DebugType=None -r osx-arm64
