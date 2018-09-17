# ExpenseClaim
Design using ASP.net core 2.1
This application has sql db as backend repository using EF core.  
If application to be tested end to end then please open command prompt, go to ExpenseClaim folder and run dotnet ef database update -v
This will create database and required tables using migration (20180914140058_InitialCreate.cs)
Run the application.  The startup.cs call seed that will create initial records in the database on initial run.
I have used Postman for testing the API
There are two HTTPGET request to get a. single Expense and b. List of Expenses
There is on POST request to add the claim.
PN - The database has two tables CostCenter and Expenses with one to Many Referencial Integration
Any validation errors such as 0 or blank amount will raise badrequest status code
Any unhandle exception such as invalid xml nodes will generate 500 status code.  This exception is captured at startup.cs as UseExceptionHandler
Object mapping is done with automapper.
Using inbuild dependency injection of Core 2.1
Unit test project using xUnit and moq

This could be further enhance to include EF Core Identity with JWT token generation.  On login the token would then be send to user and every user request to be validated against the token.

Thank you
