FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["puertstest/puertstest.csproj", "puertstest/"]
RUN dotnet restore "puertstest/puertstest.csproj"
COPY . .
WORKDIR "/src/puertstest"
RUN dotnet build "puertstest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "puertstest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "puertstest.dll"]
