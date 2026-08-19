# JapkoJudge v0.1.0
JapkoJudge is a website that allows users to challenge themselves with programming exercises. After Signing Up/Logging in user can submit their solutions to programming problems, or create their own problems for others to solve!
Currently, project only has it's skeleton - There's no styling and there are almost no security features - but the service works.
## Technology
This project was made using ASP.NET and Next.js mainly, along with PostgreSQL and Docker. 
- API endpoints are handled by **C#** backend with minimal API
- Frontend with basic routing is made with **Next.js**
- All necessary data is stored in **PostgreSQL** database
- **Docker** connects everything and also allows remote code execution (to check solutions) 
### Backend:
Backend - ASP.NET project, it's responsible mainly for setting up some services and functionalities like JWT tokens or password hashing, but it's main task is to send data to frontend with minimal api style endpoints. It's also responsible for managing database with **Entity framework**
<br>
Program.cs:
```c#
builder.Services.AddDbContext<JudgeDbContext>
...
app.Urls.Add("http://0.0.0.0:8001"); 
...
app.MapSubmissionsEndpoint();
app.MapUserEndpoint();
```
Example of api endpoint:
```c#
app.MapGet("api/languages",async(JudgeDbContext db) =>
        {
            try{
                var languages=await db.Languages.ToListAsync();
                return Results.Ok(new{languages=languages});
            }
            catch(Exception ex)
            {
                return Results.BadRequest(new{message=$"err {ex}"});
            }
        }).RequireAuthorization();
```
### Frontend:
  Next.js app, it's role is to handle user interaction with website, and to send user requests to backend to proceed them. It has filebased routing that allows to navigate pretty easily through all the project's subpages. right now every page follows the same base structure:
  - page.tsx - main page component, responsible both for actions and components.
  - actions.tsx - server component
  - Component.tsx - client's component
Example of that would be:
<br>

Component(client):
```ts
const response=await GetSubmissions(id);
    if (!response.success){
        return <p>Couldnt get Submissions.</p>
    }
    
    return <ShowSubmissions id={id} own_id={OwnId} submissions={response.submissions}/>;
```
Actions(server):
```ts
const response= await fetch(`${process.env.BACKEND_URL}/api/submissions/get`,{
        "method":"GET",
        "headers":{
            "Authorization":`Bearer ${token}`
        }
    });
    
    if (!response.ok){
        
        const err=await response.text();
        return{success:false,error:err};
    }
```
### Db:
THis project's PostgreSQL database consists of 5 tables as in the picture:
<img width="600" height="513" alt="image" src="https://github.com/user-attachments/assets/7998d9ef-9206-48ee-9ba3-84e8a6c1f09d" />
<br>
It uses postgres:18.3-alpine as the base image. Database operations are handled by backend and Entity framework mainly.

## Running this project
In order to run this project you need to have Docker installed.
Linux:
```bash
mkdir project
cd project
git clone https://github.com/JabloZ/JapkoJudge
docker compose up
```
after that, you need to configure .env file. 
Example: 
```env
DB_USER=postgres
DB_PASSWORD=password
DB_NAME=db
DB_HOST=db
DB_PORT=5432
JWT__KEY=Your own private jwt key
JWT__ISSUER=jwtkeyname
JWT__AUDIENCE=jwtkeyname
```
## Structure
Project is split into 3 main folders - backend (C#, asp.net project), frontend with (next app) and db
```
├── src/
│   ├── backend/
│   │   └── WebBackend/
│   │       ├── Api/
│   │       │   ├── ChallengesEndpoint.cs
│   │       │   ├── LoginEndpoint.cs
│   │       │   ├── RegisterEndpoint.cs
│   │       │   ├── SubmissionsEndpoint.cs
│   │       │   └── UserEndpoint.cs
│   │       ├── Code/
│   │       │   ├── DockerCodeRunner.cs
│   │       │   └── LanguageRunner.cs
│   │       ├── Dto/
│   │       ├── Migrations/
│   │       ├── Models/
│   │       │   ├── Challenges.cs
│   │       │   ├── ChallengesLanguages.cs
│   │       │   ├── Languages.cs
│   │       │   ├── Submissions.cs
│   │       │   └── User.cs
│   │       ├── Dockerfile
│   │       ├── JudgeDbContext.cs
│   │       ├── Program.cs
│   │       └── appsettings.json
│   ├── db/
│   │   └── Dockerfile
│   └── frontend/
│       ├── app/
│       │   ├── admin/
│       │   ├── challenge/
│       │   ├── create_challenge/
│       │   ├── login/
│       │   ├── register/
│       │   ├── submissions/
│       │   └── users/
│       │       ├── ranking/
│       │       └── [username]/
│       ├── lib/
│       │   ├── ChallengeCard.tsx
│       │   ├── ClassTypes.tsx
│       │   └── session.ts
│       ├── Dockerfile
│       └── package.json
├── docker-compose.yml
├── WebBackend.sln
└── README.md
```

## Functionalities
- Signing up and signing in along with jwt session ✅
- Separation for users that are not logged in ✅
- Full CRUD of creating challenges ✅
- Simple admin panel to verify challenges and set their difficulty ✅
- Submitting solutions by users ✅
- Simple ranking by accepted solutions ✅
- very simple user interface that allows these functionalities without writing api requests by hand ✅
- Executing code remotely in docker container ✅
- User profile with CRUD for profiles ❌
- Proper modern user friendly interface ❌
- Handling bad and malicious inputs ❌
- Support for various languages ❌
- Advanced and complex test system for challenge makers ❌
- Searching for challenges based on difficulty ❌
- Many functionalities that are available on websites like leetcode or codewars

### Future
  In the future I will consider this project finished after i complete todo list that's above, but also there are functionalities that i would love to add if I will have the chance to do so. The sole purpose of this project was to learn c#'s web side, but also next.js. Maybe in the future this project will be released as fully functioning website where you can actually learn to code step by step! Two main inspirations for this project were my university's site for code judging "ZawodyWeb" and also CodeWars.
