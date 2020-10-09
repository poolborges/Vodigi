#!/bin/bash
#database=Vodigi
wait_time=15s
password=P@55w0rd

# wait for SQL Server to come up
echo initi database will start in $wait_time...
sleep $wait_time

echo Creating Vodigi 5.0 database...
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/Vodigi 5.0 Master DB Scripts.sql"

echo Populate Vodigi 5.0 database...
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/Vodigi 5.0 Master DB Data Population Scripts.sql"

echo Creating VodigiLog 5.0 database...
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/VodigiLogs 5.0 Master DB Scripts.sql"

echo Upgrade Vodigi database 5.0 to 5.5
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/Vodigi 5.0 to 5.5 - DB Upgrade Scripts.sql"

echo Upgrade Vodigi database 5.5 to 6.0
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/Vodigi 5.5 to 6.0 - DB Upgrade Scripts.sql"

echo Vodigi 6.0 - Initial Account and User Admin:Admin
/opt/mssql-tools/bin/sqlcmd -S 0.0.0.0 -U sa -P $password -i "./sql/Vodigi 6.0 - Initial Account Scripts.sql"