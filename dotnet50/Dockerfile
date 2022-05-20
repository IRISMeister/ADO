FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
ENV ISC_PACKAGE_INSTALLDIR /usr/irissys
ENV LIBDIR lib
ENV ISC_LIBDIR ${ISC_PACKAGE_INSTALLDIR}/dev/dotnet/bin/Core21
#ENV ISC_LIBDIR ${ISC_PACKAGE_INSTALLDIR}/dev/dotnet/bin/net5.0
WORKDIR /ADO
COPY --from=containers.intersystems.com/intersystems/iris-community:2021.2.0.651.0 $ISC_LIBDIR/*.nupkg $LIBDIR/
#COPY --from=containers.intersystems.com/intersystems/iris-community:2022.1.0.199.0 $ISC_LIBDIR/*.nupkg $LIBDIR/
