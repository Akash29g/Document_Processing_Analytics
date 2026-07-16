# Built in the workflow AFTER `dotnet ef migrations bundle` produces ./efbundle
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app
COPY efbundle .
ENTRYPOINT ["/bin/sh", "-c", "./efbundle --connection \"$ConnectionStrings__Default\""]
