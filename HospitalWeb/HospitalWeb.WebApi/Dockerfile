#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /api
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HospitalWeb.WebApi/HospitalWeb.WebApi.csproj", "HospitalWeb.WebApi/"]
COPY ["HospitalWeb.WebApi.Services/HospitalWeb.WebApi.Services.csproj", "HospitalWeb.WebApi.Services/"]
COPY ["HospitalWeb.Domain/HospitalWeb.Domain.csproj", "HospitalWeb.Domain/"]
RUN dotnet restore "HospitalWeb.WebApi/HospitalWeb.WebApi.csproj"
COPY . .
WORKDIR "/src/HospitalWeb.WebApi"
RUN dotnet build "HospitalWeb.WebApi.csproj" -c Release -o /api/build

FROM build AS publish
RUN dotnet publish "HospitalWeb.WebApi.csproj" -c Release -o /api/publish

FROM base AS final
WORKDIR /api
COPY --from=publish /api/publish .
ENTRYPOINT ["dotnet", "HospitalWeb.WebApi.dll"]