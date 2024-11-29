# Database setup

1. Open appsetting.json file and change the connection string to the connection string of your local MSSQL database.

```json
"ConnectionStrings": {
  "LunaEdgeDb": "Your connection string"
}
```

2. Open the project in the terminal. Input the following commands to create the tables in your local database and fill them with initial data:

```
dotnet ef database update InitialCreate
