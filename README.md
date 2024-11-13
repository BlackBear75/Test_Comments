Test_Comments
Test_Comments is a web application that allows users to share their thoughts, comment on posts, and interact with each other through responses. The application supports uploading text files and images, which makes the interaction more interactive.

Technologies
Frontend: Angular
Backend: ASP.NET Core (C#)
Database: Microsoft SQL Server (MS SQL)
Functional
User profile: Users can create a profile, edit their data and view their own posts.
Post Creation: Users can create posts to share thoughts or other textual content.
Commenting: Other users can leave comments on posts.
File uploads: Users can upload text files and images while creating posts.
Project settings
Requirements:

Node.js and Angular CLI (ng) for frontend.
.NET SDK for the backend.
SQL Server for database deployment.
Local launch of the project
Clone the repository:

bash
Copy code
git clone git@github.com:BlackBear75/Test_Comments.git
Go to the project directory:

bash
Copy code
cd Test_Comments
Database connection settings:

Open appsettings.Development.json or create it in the project root.
Add your database connection string:
json
Copy code
{
 "ConnectionStrings": {
 "DefaultConnection": "Server=localhost;Database=Test_Comments_Local;User Id=your_user;Password=your_password;Encrypt=False;TrustServerCertificate=True"
 }
}
Running the backend:
Navigate to the Test_Comments folder.
Run the command to run:
Copy code
dotnet run
The backend will be available at http://localhost:7092.
Installing and running the frontend:

Navigate to your Angular project folder (usually Test_Comments/ClientApp or similar).
Install dependencies:
Copy code
npm install
Start the frontend:
Copy code
ng serve
The frontend will be available at http://localhost:4200.
Access to the application:
Frontend: http://localhost:4200
Backend API: http://localhost:7092
