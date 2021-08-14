# F# Safe Stack MVU Code for Scheduler Comparison
# SAFE Template
This project was used with [SAFE Stack](https://safe-stack.github.io/)'s [Minimal template](https://safe-stack.github.io/docs/template-overview/)  to generate a full-stack web application. It was created using the dotnet.

## Install pre-requisites
You'll need to install the following pre-requisites in order to build SAFE applications

* The [.NET Core SDK](https://www.microsoft.com/net/download) 3.1
* [npm](https://nodejs.org/en/download/) package manager & Node LTS

## Starting the Application
#### Both are required to run correctly
To Start the Server:
```bash
cd src\Server\
dotnet run     # Start up the Server with a localhost:8085 link
```

To Start the client:
```bash
npm install    # To get Node-related dependencies
npm run start
```

To Run the Client Tests
```bash
npm test # A dedicated self-creataed test command via SAFE Stack Docs
```

To Run the Client Tests
```bash
#Assuming that it is on the project root folder
dotnet run -p tests/Server #Dedicated dotNET test command
# All results will be via Console
```

Open a browser to `http://localhost:8080` to view the site.

## SAFE Stack Documentation
If you want to know more about the full Azure Stack and all of its components (including Azure) visit the official [SAFE documentation](https://safe-stack.github.io/docs/).

You will find more documentation about the used F# components at the following places:
* [Saturn](https://saturnframework.org/docs/)
* [Elmish](https://elmish.github.io/elmish/)
