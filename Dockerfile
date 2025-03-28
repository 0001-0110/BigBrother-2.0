FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.ico ./
COPY *.sln *.csproj ./
RUN dotnet restore
COPY *.cs **/*.cs ./
RUN dotnet publish -c Release --property:PublishDir=out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /src/out/ ./
COPY Data/ ./Data/
ENTRYPOINT [ "dotnet", "BigBrother.dll" ]
