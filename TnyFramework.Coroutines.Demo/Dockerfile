FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TnyFramework.Coroutine.Demo/TnyFramework.Coroutine.Demo.csproj", "TnyFramework.Coroutine.Demo/"]
RUN dotnet restore "TnyFramework.Coroutine.Demo/TnyFramework.Coroutine.Demo.csproj"
COPY . .
WORKDIR "/src/TnyFramework.Coroutine.Demo"
RUN dotnet build "TnyFramework.Coroutine.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TnyFramework.Coroutine.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TnyFramework.Coroutine.Demo.dll"]
