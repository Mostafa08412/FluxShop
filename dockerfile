FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0 as build

WORKDIR /build

COPY ["./FluxStore.slnx" , "./"]
COPY ["./src/FluxStore.Api/FluxStore.Api.csproj","./src/FluxStore.Api/FluxStore.Api.csproj"]

RUN dotnet restore "FluxStore.slnx"

COPY . .

RUN dotnet publish --no-restore "./src/FluxStore.Api/FluxStore.Api.csproj" -c Release -o ../release


FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 as final

WORKDIR /app

COPY --from=build ../release . 

ENV ASPNETCORE_URLS=http://+:5089
EXPOSE 5089

ENTRYPOINT [ "dotnet","FluxStore.Api.dll" ]
