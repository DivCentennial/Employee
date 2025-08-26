#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY [".nuget/NuGet.config", "./"]
COPY ["ApiService/ApiService.csproj", "ApiService/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["DataCarrier/DataCarrier.csproj", "DataCarrier/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Repository.Contracts/Repository.Contracts.csproj", "Repository.Contracts/"]
COPY ["DataModel/DataModel.csproj", "DataModel/"]
RUN dotnet restore "ApiService/ApiService.csproj"  --configfile ./NuGet.config
COPY . .
WORKDIR "/src/ApiService"
RUN dotnet build "ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MariApps.PAL.Purchase.Employee.ApiService.dll"]