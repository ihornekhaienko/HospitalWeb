#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /mvc
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HospitalWeb.Mvc/HospitalWeb.Mvc.csproj", "HospitalWeb.Mvc/"]
COPY ["HospitalWeb.Domain/HospitalWeb.Domain.csproj", "HospitalWeb.Domain/"]
COPY ["HospitalWeb.Mvc.Services/HospitalWeb.Mvc.Services.csproj", "HospitalWeb.Mvc.Services/"]
RUN dotnet restore "HospitalWeb.Mvc/HospitalWeb.Mvc.csproj"
COPY . .
WORKDIR "/src/HospitalWeb.Mvc"
RUN dotnet build "HospitalWeb.Mvc.csproj" -c Release -o /mvc/build

FROM build AS publish
RUN dotnet publish "HospitalWeb.Mvc.csproj" -c Release -o /mvc/publish

FROM base AS final
WORKDIR /mvc
COPY --from=publish /mvc/publish .
ENTRYPOINT ["dotnet", "HospitalWeb.Mvc.dll"]