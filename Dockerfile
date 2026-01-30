# Top-level Dockerfile to build the LMS solution
# This builds the LMS project located in the LMS/ subfolder and produces an image at repo root

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

# Copy project file(s) and restore as distinct layers
COPY LMS/LMS.csproj LMS/
RUN dotnet restore "LMS/LMS.csproj"

# Copy everything and build
COPY . .
RUN dotnet publish "LMS/LMS.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "LMS.dll"]
