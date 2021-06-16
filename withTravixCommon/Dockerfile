FROM mcr.microsoft.com/dotnet/runtime-deps:5.0

WORKDIR /app
COPY . ./

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

ENTRYPOINT ["./withTravixCommon.WebService"]