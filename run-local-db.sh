#!/usr/bin/env bash

SA_PASSWORD="pls?noh4ck54ll0w3d!"

echo "Pulling the latest SQL Server image..."
podman pull mcr.microsoft.com/mssql/server:2019-latest

if podman container exists sqlserver; then
    echo "A container named 'sqlserver' already exists, removing it..."
    podman container stop sqlserver
    podman container rm sqlserver
    if [ "$1" == "reset" ]; then
        podman volume rm sqlserver && echo "removed 'sqlserver' volume"
    fi
fi

echo "Starting SQL Server container..."
podman run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=${SA_PASSWORD}" \
    -p 1433:1433 --name sqlserver \
    -v "sqlserver:/var/opt/mssql" \
    -d mcr.microsoft.com/mssql/server:2019-latest

if [ $? -eq 0 ]; then
    echo
    echo "Connection string:"
    echo "Server=localhost,1433;Database=master;User Id=SA;Password=${SA_PASSWORD};Encrypt=false;"
    echo "SQL Server now running with Podman."
else
    echo "Failed to start SQL Server container."
fi
