#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && apt-get install wkhtmltopdf -y --no-install-recommends && apt-get clean
RUN mkdir /Linux && ln -s /usr/bin/wkhtmltopdf /Linux/wkhtmltopdf

USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Html2Pdf/Html2Pdf.csproj", "Html2Pdf/"]
RUN dotnet restore "./Html2Pdf/./Html2Pdf.csproj"
COPY . .
WORKDIR "/src/Html2Pdf"
RUN dotnet build "./Html2Pdf.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Html2Pdf.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Html2Pdf.dll"]