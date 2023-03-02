FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Delta.Application/Delta.Application.csproj", "Delta.Application/"]
RUN dotnet restore "Delta.Application/Delta.Application.csproj"
COPY . .
WORKDIR "/src/Delta.Application"
RUN dotnet build "Delta.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Delta.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Delta.Application.dll"]
