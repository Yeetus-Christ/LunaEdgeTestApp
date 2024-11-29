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
```

# Api Documentation

## Users controller

| Method   | URL                                      | Description                              | Body example |
| -------- | ---------------------------------------- | ---------------------------------------- | ---------------------------------------- |
| `POST`   | `/api/Users/login`                       | Login with username/email and password.  | <pre>{<br>  "username": "string",<br>  "password": "string"<br>}</pre> |
| `POST`   | `/api/Users/register`                    | Register with username, email and password. | <pre>{<br>  "username": "string",<br>  "email": "string",<br>  "password": "string"<br>}</pre> |

## Tasks controller

All endpoints in Tasks controller require a bearer JWT token in `Authorization` header to function

| Method   | URL                                      | Description                              | Body example |
| -------- | ---------------------------------------- | ---------------------------------------- | ---------------------------------------- |
| `POST`   | `/api/Tasks`                       | Create a task.  | <pre>{<br>  "title": "string",<br>  "description": "string",<br>  "dueDate": "2024-11-29T12:39:08.557Z",<br>  "status": 0,<br>  "priority": 0<br>}</pre> |
| `GET`   | `/api/Tasks?[status]&[dueDate]&[priority]&[sortBy]&[page]&[pageSize]`                    | Get list of tasks. | - |
| `GET`   | `/api/Tasks/{id}`                       | Get a specific task by id.  | - |
| `PUT`   | `/api/Tasks/{id}`                    | Update a specific task. | <pre>{<br>  "title": "string",<br>  "description": "string",<br>  "dueDate": "2024-11-29T12:39:08.557Z",<br>  "status": 0,<br>  "priority": 0<br>}</pre> |
| `DELETE`   | `/api/Tasks/{id}`                       | Delete a specific task.  | - |
