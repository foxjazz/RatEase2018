FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 49296
EXPOSE 44336

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY ServiceFroos/ServiceFroos.csproj ServiceFroos/
RUN dotnet restore ServiceFroos/ServiceFroos.csproj
COPY . .
WORKDIR /src/ServiceFroos
RUN dotnet build ServiceFroos.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ServiceFroos.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ServiceFroos.dll"]
