#!/bin/bash
# scripts/entrypoint.sh
# Script de entrada personalizado para SQL Server

echo "Starting SQL Server initialization..."

# Iniciar SQL Server en background
/opt/mssql/bin/sqlservr &

# Esperar a que SQL Server esté listo
echo "Waiting for SQL Server to start..."
sleep 30

# Verificar que SQL Server está corriendo
for i in {1..30}; do
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1; then
        echo "SQL Server is ready!"
        break
    fi
    echo "Waiting for SQL Server... attempt $i"
    sleep 5
done

# Ejecutar script de inicialización
echo "Executing database initialization script..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-entrypoint-initdb.d/init-database.sql

if [ $? -eq 0 ]; then
    echo "Database initialization completed successfully!"
else
    echo "Database initialization failed!"
    exit 1
fi

# Mantener SQL Server corriendo en foreground
wait