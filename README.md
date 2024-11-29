# Project setup

## Without docker

1. Open appsetting.json file and change the connection string to the connection string of your local MSSQL database (If the database specified in connection string doesn't exist, starting the app will automatically create it and migrate tables):

```json
"ConnectionStrings": {
  "LunaEdgeDb": "Your connection string"
}
```

After that, you can start the application.

## With docker

1. Open the folder containing local repository in the terminal. Then execute the following command:

```
docker-compose up --build
```

This command will create ASP.NET Core Web API and MSSQL containers and automatically start them. When Web API starts up, it will automatically create database in MSSQL container and migrate tables. After that, you can access endpoints on "https://localhost:8081/". Additionally, you can go to "https://localhost:8081/swagger/index.html" to check API documentation

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

# Explanation of Architecture and Design Choices

The architecture of the application is based on well-established design patterns and best practices to ensure maintainability, scalability, and separation of concerns. The key design choices include the use of DTOs (Data Transfer Objects), Repository Pattern, Service Pattern, and Migrations.

## Use of DTOs (Data Transfer Objects)

DTOs are used as an intermediary between the application's internal data models and external systems (such as APIs or databases).

## Repository Pattern

The Repository Pattern is implemented to abstract the data access logic and provide a clean API for the service layer to interact with the database. The repository serves as a collection-like interface for accessing domain objects.

## Service Pattern

The Service Pattern is used to encapsulate business logic within service classes. The service layer acts as an intermediary between the controller (or API endpoint) and the repository, orchestrating the necessary operations and ensuring that the logic is clean and maintainable.

## Migrations

Migrations are used to manage the evolution of the database schema over time. With Entity Framework Core (EF Core), migrations allow you to version control your database schema and ensure that the database structure is synchronized with the application code.
