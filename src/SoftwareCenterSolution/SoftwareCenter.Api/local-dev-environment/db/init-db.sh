#!/bin/bash

# Create both the software and db databases if they don't exist
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    SELECT 'CREATE DATABASE software'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'software')\gexec
    
    SELECT 'CREATE DATABASE db'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'db')\gexec
EOSQL

echo "Databases 'software' and 'db' are ready!"
