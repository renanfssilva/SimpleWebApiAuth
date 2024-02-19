# BookStore Web API

## Overview
This project is a simple web application that serves as a CRUD (Create, Read, Update, Delete) API for managing books in a bookstore. The application is developed using .NET C#, ASP.NET MVC, Web API, and MongoDB, adhering to Clean Architecture principles and Test-Driven Development (TDD) methodologies.

## User Story
As a book enthusiast and store owner, I want to manage the inventory of my bookstore efficiently. I need a system where I can add new books, update existing ones, remove books that are no longer available, and retrieve information about all the books in my inventory. Additionally, I need a secure way to manage user authentication for accessing the system.

## Thought Process
- **Project Selection**: I chose to develop a CRUD API for a bookstore because it aligns with the requirements of the technical interview exercise, and it allows me to demonstrate my understanding of Clean Architecture, TDD, and API development.
- **Database Selection**: I opted for MongoDB as the data storage solution due to its flexibility and scalability. It allows me to store book records efficiently and retrieve them quickly using its document-oriented structure.
- **Model Design**: I designed the Book model to contain essential information about a book, including its name, price, category, and author. Each book has a unique identifier.
- **API Development**: I implemented the CRUD operations for managing books via API endpoints. Additionally, I created endpoints for user creation, login, and authentication to ensure secure access to the system.
- **Test-Driven Development**: I followed TDD methodologies by writing unit tests for all components of the application, including the data access layer, business logic layer, and API endpoints. This ensures that each component functions as expected and maintains its integrity throughout the development process.

## How to Run

To run the application locally using Docker, follow these steps:

1. Clone the repository to your local machine:
```bash
git clone https://github.com/renanfssilva/SimpleWebApiAuth.git
cd SimpleWebApiAuth
```
2. Build the Docker images:
```bash
docker compose -f docker-compose.yml build
```

3. Start the Docker containers:
```bash
docker compose -f docker-compose.yml up
```

4. Access the API endpoints:

Once the containers are up and running, you can access the API endpoints using a tool like Postman or Swagger. The API documentation should be available at `http://localhost:<port>/swagger/index.html`, where `<port>` is the port mapped to the API container.

## API Endpoints
POST /signup: Registers a new user.

POST /login: Logs in a user and generates a token for authentication.

POST /admin: Give the "admin" role to the curent user.

GET /users: Retrieves a list of all users.

GET /users/current: Retrieves details of the current user.

GET /books: Retrieves a list of all books.

GET /books/{id}: Retrieves details of a specific book by ID.

POST /books: Creates a new book.

PUT /books/{id}: Updates details of a specific book by ID.

DELETE /books/{id}: Deletes a specific book by ID.

## Conclusion
This project demonstrates my ability to develop a clean and maintainable web application using .NET C#, ASP.NET MVC, Web API, and MongoDB. By adhering to Clean Architecture principles and TDD methodologies, I have ensured that the application is well-structured, scalable, and easy to maintain. Thank you for considering my submission.
