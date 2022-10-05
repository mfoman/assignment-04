# How to run datbase

Run all commands from root folder of project where `docker-compose.yml` lives.

## Start

`docker compose up -d`

### New server

Databse lives at `localhost:5432`
And has following login:

User: `postgres`
Pass: `postgrespw`

You can go to pgadmin4 at `localhost:8080`
Login is:

User: `admin@localhost.com`
Pass: `admin`

When connecting new server, use:

Connection: `host.docker.internal`
Port: `5432`
User and pass is same as postgres above.


## Stop

`docker compose down`
