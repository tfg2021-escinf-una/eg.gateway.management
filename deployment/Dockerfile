FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG Configuration=Release

COPY . .

#Compiling the application with the .NET SDK.

RUN dotnet restore ./src/*.sln
RUN dotnet build ./src/*.sln --no-restore --nologo -v normal
RUN dotnet publish ./src/EG.Gateway.Microservice.csproj -c ${Configuration} -o /publish/app --no-build --nologo -v normal

#Build application with runtime .NET image

FROM mcr.microsoft.com/dotnet/aspnet:6.0

ENV ASPNETCORE_URLS="http://*:8081"
EXPOSE 8081/tcp

RUN mkdir -p /var/app \
    && groupadd -g 2000 container_group \
    && useradd -u 1000 container_user \
    && chown -R container_user:container_group /var/app
    
COPY --from=build-env /publish/app /var/app
WORKDIR /var/app
USER container_user

ENTRYPOINT ["dotnet", "EG.Gateway.Microservice.dll"]