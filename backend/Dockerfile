FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore

WORKDIR /sln

COPY ./*.sln ./

# Copy the main source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Restore the NuGet packages
RUN dotnet restore

COPY . .

FROM restore AS publish
RUN dotnet publish --configuration "Release" --no-restore -o "/app" ./src/Reacto.Web/Reacto.Web.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
EXPOSE 11111
EXPOSE 30000
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT [ "./Reacto.Web" ]
