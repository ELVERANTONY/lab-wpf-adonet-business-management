#!/usr/bin/env bash
set -euo pipefail

if [[ ! -f .env ]]; then
  echo "Falta .env. Copia .env.example y establece una contraseña segura." >&2
  exit 1
fi

set -a
source .env
set +a

SQLCMD=/opt/mssql-tools18/bin/sqlcmd
for attempt in {1..30}; do
  if docker exec neptuno-sqlserver "$SQLCMD" -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; then break; fi
  if [[ "$attempt" == 30 ]]; then echo "SQL Server no respondió a tiempo." >&2; exit 1; fi
  sleep 2
done

docker exec neptuno-sqlserver "$SQLCMD" -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -i /scripts/01_Init_NeptunoDB.sql
docker exec neptuno-sqlserver "$SQLCMD" -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b -i /scripts/02_Procedimientos_Almacenados.sql
echo "NeptunoDB y sus procedimientos quedaron listos."
