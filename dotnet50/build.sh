#!/bin/bash
cp -fR /source/ADO/* .
dotnet add ADO.csproj package MathNet.Numerics --version 5.0.0
dotnet add ADO.csproj package InterSystems.Data.IRISClient -s .
dotnet restore ADO.csproj
dotnet publish ADO.csproj -c debug -o /app

echo "dotnet /app/ADO.dll to run"