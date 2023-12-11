#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Test/Test.Client/Test.Client.csproj", "Test/Test.Client/"]
RUN dotnet restore "Test/Test.Client/Test.Client.csproj"
COPY . .
WORKDIR "/src/Test/Test.Client"
RUN dotnet build "Test.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Test.Client.dll"]