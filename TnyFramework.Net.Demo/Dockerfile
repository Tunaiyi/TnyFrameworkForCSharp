FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TnyFramework.Net.Demo/TnyFramework.Net.Demo.csproj", "TnyFramework.Net.Demo/"]
RUN dotnet restore "TnyFramework.Net.Demo/TnyFramework.Net.Demo.csproj"
COPY . .
WORKDIR "/src/TnyFramework.Net.Demo"
RUN dotnet build "TnyFramework.Net.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TnyFramework.Net.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TnyFramework.Net.Demo.dll"]
