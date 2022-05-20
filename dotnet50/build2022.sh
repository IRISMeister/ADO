#!/bin/bash
cp -fR /source/ADO/* .
cp /source/dotnet50/ADO2022.csproj ./ADO.csproj
dotnet restore ADO.csproj
dotnet publish ADO.csproj -c debug -o /app

echo "dotnet /app/ADO.dll to run"