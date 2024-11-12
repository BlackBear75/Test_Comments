Test_Comments
Test_Comments is a web application that allows users to share their thoughts, comment on posts, and interact with others through replies. 
The application supports uploading text files and images for a more engaging experience.

Technologies
Frontend: Angular
Backend: ASP.NET Core (C#)
Database: Microsoft SQL Server (MS SQL)

Features
User Profile: Users can create a profile, edit their information, and view their posts.
Post Creation: Users can create posts to share their thoughts or other textual content.
Commenting: Other users can reply to posts by leaving comments.
File Uploads: Users can upload text files and images when creating posts, enhancing the content.

Project Setup
Requirements
Node.js and Angular CLI for the frontend.
.NET SDK for the backend.
SQL Server or Docker to deploy the database.

Running the Project with Docker
Clone the repository: git clone git@github.com:BlackBear75/Test_Comments.git
Enter the Test_Comments directory: cd Test_Comments
Environment Setup: The docker-compose.yml file is preconfigured with the necessary environment variables.

Start the Project: Use Docker Compose to build and start the project with docker-compose up --build

This will start three services:
SQL Server on port 1433
Backend on port 7092
Frontend on port 8080

Access the Application
Frontend: http://localhost:8080
Backend API: http://localhost:7092
