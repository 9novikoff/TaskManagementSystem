# Task Management System

## Overview  
Task Management System is a simple and secure app for organizing tasks. It lets users log in, create and update tasks. Features include task filtering, sorting, and pagination, making it easy to stay organized.

## Setup Instructions  

### Prerequisites  
- .NET SDK (version 8.0.0 or later)  
- Entity Framework Core  
- PostgreSQL
- (Optional) Docker for containerized run  

### Steps to Run Locally  
1. Clone the repository:  
   ```bash
   git clone (https://github.com/9novikoff/TaskManagementSystem
   cd TaskManagementSystem/TaskManagementSystem
2. Set up the database:

- Update the connection string in appsettings.json.

- Apply migrations:
  ```
  dotnet ef database update
  ```
  
3. Build and run the project:
  ```bash
  dotnet build
  dotnet run
  ```

4. Open the application in your browser at https://localhost:5218 (or as specified in your Properties/lauchSettings.json file).

## Endpoints

The following table summarizes the available API endpoints:

| Method | URL                         | Description                                   |
|--------|-----------------------------|-----------------------------------------------|
| POST   | /users/register              | Register a new user                          |
| POST   | /users/login                 | Login an existing user and retrieve a JWT token |
| POST   | /tasks                       | Create a new task                            |
| GET    | /tasks                       | Retrieve a list of tasks                     |
| GET    | /tasks/{taskId:guid}         | Get a specific task by ID                    |
| PUT    | /tasks/{taskId:guid}         | Update an existing task                      |
| DELETE | /tasks/{taskId:guid}         | Delete a task by ID                          |

---

### User Endpoints

#### **POST /users/register**
Registers a new user.

**Request Body**:
```json
{
  "username": "exampleuser",
  "email": "example@example.com",
  "password": "Password123_"
}
```
**Responses**:

- 200 OK: If registration is successful.
- 403 Forbidden: If registration fails.

#### **POST /users/login**
Logs in an existing user and returns a JWT token.

**Request Body**:
```json
{ 
  "usernameOrEmail": "example@example.com",
  "password": "Password123_"
}
```

**Response**:

- 200 OK: Returns the JWT token if login is successful.
- 403 Forbidden: If login fails (invalid credentials).
  
### Task Endpoints

#### **POST /tasks**
Creates a new task for the authenticated user.

**Request Body**:
```json
{
  "title": "Task Title",
  "description": "Task description",
  "dueDate": "2024-12-31T23:59:59",
  "priority": 0,
  "status": 0
}
```

**Response**:

- 200 OK: If the task is created successfully.
- 400 Bad Request: If the request body is invalid or missing required fields.

#### **GET /tasks**
Retrieves a list of tasks for the authenticated user. Supports filtering, sorting, and pagination.

**Query Parameters**:

- filter: Filters tasks by status, priority, or date range.
- sort: Sorts tasks by a specified column.
- pagination: Limits the number of results per page and specifies the page number.

**Response**:

- 200 OK: Returns a list of tasks matching the filter criteria.
- 400 Bad Request: If the request is invalid.
  
#### **GET /tasks/{taskId:guid}**
Fetches a specific task by its ID for the authenticated user.

**Path Parameter**:

- taskId: The GUID of the task to retrieve.

**Response**:

- 200 OK: Returns the task if found.
- 400 Bad Request: If the task ID is invalid or the task does not exist.

#### **PUT /tasks/{taskId:guid}**
Updates an existing task.

**Path Parameter**:

- taskId: The GUID of the task to update.

**Request Body**:
```json
{
  "title": "Updated Task Title",
  "description": "Updated description",
  "dueDate": "2024-12-31T23:59:59",
  "priority": 1,
  "status": 1
}
```

**Response**:

- 200 OK: If the task is updated successfully.
- 400 Bad Request: If the request is invalid or the task does not exist.

#### **DELETE /tasks/{taskId:guid}**
Deletes a specific task by its ID.

**Path Parameter**:

- taskId: The GUID of the task to delete.

**Response**:

200 OK: If the task is deleted successfully.
400 Bad Request: If the task does not exist or the request is invalid.

## Architecture 
This project follows a layered architecture to maintain separation of concerns, scalability, and maintainability. Below are the key design choices:

### 1. Authentication and Authorization
- **JWT Authentication**: The application uses JWT for user authentication. The JWT token is validated using a symmetric security key, issuer, and audience as configured in the application settings.
- **Authorization**: The `Authorize` attribute is applied to TaskController or actions that require user authentication, ensuring that only authenticated users can access protected resources.

### 2. Data Access Layer
- **Entity Framework Core**: The application uses EF Core to interact with the PostgreSQL database. The `TaskDbContext` class is responsible for the database connection and managing entity models. This choice simplifies database operations and supports migrations.
- **Repository Pattern**: Repositories are used to abstract away database operations. The `UserRepository` and `TaskRepository` interact with the database and abstract Service layer from database communication.

### 3. Business Logic Layer
- **Services**: The `UserService` and `TaskService` classes handle the core business logic of the application. These services interact with repositories to perform operations on the data and return results. The service layer decouples the business logic from the API layer.
- **FluentValidation**: The application uses FluentValidation to validate incoming data for user registration, login, and task management. Validators like `LoginDtoValidator` and `CreateUserTaskDtoValidator` ensure that user input is validated consistently.
- **Result pattern**: To prevent slowdowns caused by exceptions services return results that allow to handle different scenarios efficiently

### 5. AutoMapper
- AutoMapper is used to map between DTOs and domain models. This simplifies the mapping process and ensures that only the necessary fields are transferred between layers. The `MappingProfile` class holds the configuration for mapping between models and DTOs.

## Explanation for some of my choices
https://docs.google.com/document/d/12zg2mupF7myVheodjKm3iZPrv25_hl_5-zQAGXmGCMk/edit?usp=sharing









