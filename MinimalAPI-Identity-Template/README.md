# MinimalAPI Identity Template
The provided .net core 8.0 Minimal API + Identity based Authentication/Authorization starter kit is designed to expedite the development process by alleviating the need for developers to handle the complexities of an authentication system.

## Getting Started
### Prerequisites
* .Net Core 8.0 or above
* Node 18.14.0 & pnpm 8.7.4 or above
## Install
1. Clone this project to your computer.
2. Input the values for the `EmailPassword`, `ClientId`, and `ClientSecret` fields in the `appsettings.Development.json` file located at the project root.
3. Execute the command `dotnet run --launch-profile https` to start the server.
4. Navigate to the MinimalAPI-Identity-Template-FrontEnd directory and run `pnpm install` to install packages.
5. Run `pnpm -o dev` to start the front-end server.
## Usage
### .Net Core Minimal API Server
Navigate to https://localhost:7281/swagger/index.html to access the API documentation, as depicted in the image below.
![swagger.png](images%2Fswagger.png)
The endpoints include User, Role, Password, Login, Google Login, Email Login, and CSRF token.  
Take /api/identity/register as an example; its usage is as follows:    
Submit a POST request with a request body containing the following data:  
```
{
  "userName": "string",
  "email": "string", 
  "password": "string", 
  "nickName": "string"
}
```
If successful, the server will return a status code of 200 along with a JSON response similar to this:  
```
{
  "status": "string",
  "data": [
    {
      "id": "string",
      "userName": "string",
      "email": "string",
      "nickName": "string",
      "emailConfirmed": true,
      "roles": [
        "string"
      ]
    }
  ]
}
```
If unsuccessful, the server will return a status code of 400 along with a JSON response like this:  
```
{
  "status": "string",
  "erros": [
    {
      "code": "string",
      "description": "string"
    }
  ]
}
```
### Vue 3 front end view
Navigate to https://localhost:3000/ to access the front end view, as depicted in the image below.
![vue3-view.png](images%2Fvue3-view.png)
The view contains basic identity features like `Register`, `Login`, `Google login` etc...
You can use this view as a starting point of a new project or simply use it to test endpoints.  
For instance, test the login functionality:  
![login-test.gif](images%2Flogin-test.gif)