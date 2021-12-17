# CA Location Chatbot
This bot was created as a proof of concept to locate a public unonmous user in California using a live chat bot before transferring the conversation to a live agent.

![CA Location Bot](ReadMeAssets/botScreenshot.jpg)
___
## Demo and Usage
You can try out the bot yourself at [https://techlifornia.us](https://techlifornia.us).

It will:
1. Greet you
2. Make a call to ipify.org to get your ip address
3. Make a call to the back-end application to figure out where your ip address is located
    - If it is in California it will tell you where it found you to be (city and county) and ask if it is correct
    - If it is not identified or not in California it will ask where you are in California (either city or county)
        - Once you type in a city or county in California it will make a call to back-end application to search for the closest entry that exists in the list of cities and counties on the [California Department of Tax](https://www.cdtfa.ca.gov/taxes-and-fees/rates.aspx) website as of October 2021
        - If it finds a match it will tell you where it found you to be (City and County) and ask if it is correct
        - If it does not find a match it will ask you ask again where you are in California (either city or county)
4. Once you confirm the county the bot found you to be type "yes" (without the quotes) and the bot will transfer the conversation to a surprise live agent
5. The live agent is in fact a QnA Maker knowledgebase that is trained to answer specific questions about the surprise live agent
___
## Free Third-Party Technology Used
- The ip address is obtained from [ipify.org](https://ipify.org).
- The ip address location is looked up from the [MaxMind GeoLite2](https://www.maxmind.com/en/geoip2-services-and-databases) database.
- The database is downloaded as a gzip tarball so [SharpZipLib](http://icsharpcode.github.io/SharpZipLib/) was used to decompress it.
- The manual entry of city/county is looked up in [Azure Cognitive Search](https://azure.microsoft.com/en-us/services/search/).
- The live agent "Question Answering" (<em>previously QnA Maker</em>) is done using [Azure Cognitive Services for Language](https://azure.microsoft.com/en-us/services/cognitive-services/question-answering/).
- The bot theme was taken from [Nebular](https://akveo.github.io/nebular/) dark theme.
- The front-end client was written in [Angular](https://angular.io/) and [Typescript](https://www.typescriptlang.org/) in [Visual Studio Code IDE](https://code.visualstudio.com/).
- The back-end application was written in [C# 10](https://docs.microsoft.com/en-us/dotnet/csharp/) using [Dotnet 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) in [Visual Studio 2022 IDE](https://visualstudio.microsoft.com/vs/).
- Hosting happens with [Azure Static Web App](https://azure.microsoft.com/en-us/services/app-service/static/).
___
## Cloud Services Needed
You will need the following free cloud services established:
### MaxMind
1. [Sign Up](https://www.maxmind.com/en/geolite2/signup?lang=en) for GeoLite2 Free Geolocation Data
2. [Generate](https://www.maxmind.com/en/accounts/current/license-key?lang=en) a License Key
3. Write down your Account/User ID and the License Key
    - You won't have another opportunity to see that key so keep it in a safe place
### GitHub
1. If you don't already have a GitHub account, [Sign Up](https://github.com/signup) for one
2. There are a few otions to proceed:
    - Click on the "[Use this template](https://github.com/glensouza/ca-location-bot/generate)" button to generate a new repository onto your own GitHub account
    - Clone [this](https://github.com/glensouza/ca-location-bot) repository on your development workstation and if you feel like contributing to it you can create a Pull Request
    - Fork [this](https://github.com/glensouza/ca-location-bot) repository to your own GitHub account by clicking on the "Fork" button on top right of the page to continue this work with your own requirements
    - Create your own repository:
        - Create new [repository](https://github.com/new) on your GitHub account
        - Clone the new repository locally on your development workstation
        - Clone [this](https://github.com/glensouza/ca-location-bot) reporitory locally on your development workstation in a different directory
        - Copy the contents that you like from this of this repository onto your own repository's directory
        - Push your changes to the new repository
3. If you created your own repository and intend on deploying it to your own hosting environment make sure to delete the file [https://github.com/glensouza/ca-location-bot/tree/main/.github/workflows/azure-static-web-apps-wonderful-meadow-06f3bbc1e.yml](https://github.com/glensouza/ca-location-bot/tree/main/.github/workflows) as that is specific to my own Azure resource in step 5 of the **Azure** section, but before you do take a look at it and compare it to what yours look like - it might help you troubleshoot if something goes wrong.
### Azure
1. If you don't already have an Azure account, [Sign Up](https://azure.microsoft.com/en-us/free/) for a free account today
    - Includes the services we need to run the bot for free (with limits of course)
    - You will get $200 credit to try out other services that are not free for the first 30 days from your account creation
2. Create a [Resource Group](https://portal.azure.com/#create/Microsoft.ResourceGroup)
3. Create an [Application Insights](https://portal.azure.com/#create/Microsoft.AppInsights)
4. Create a [Search Service](https://portal.azure.com/#create/Microsoft.Search)
    - Save the name of the Search Service and the primary search key for later use in code

        ![Search Service](ReadMeAssets/searchKeys.jpg)

5. Create a [Static Web App](https://portal.azure.com/#create/Microsoft.StaticApp)
    - The most important part during creation selecting your GitHub repository

        ![Static Web App](ReadMeAssets/swaGitHub.jpg)
___
## Development Workstation
### Required Installed Free Software
- front-end client was written in [Angular](https://angular.io/) and [Visual Studio Code IDE](https://code.visualstudio.com/).
    - You will need to have [NodeJS](https://nodejs.org/en/) installed
    - By default it installs the [npm client](https://docs.npmjs.com/about-npm)
    - You will need to have [Angular CLI](https://angular.io/guide/setup-local) installed by typing into your terminal:
        ```
        npm install -g @angular/cli
        ```
- The back-end application was written in [C# 10](https://docs.microsoft.com/en-us/dotnet/csharp/) using [Dotnet 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and [Visual Studio 2022 IDE](https://visualstudio.microsoft.com/vs/).
    - If you install Visual Studio 2022 you should already get C# and Dotnet installed
    - You will need to have [Static Web Apps CLI](https://github.com/Azure/static-web-apps-cli) installed by typing into your terminal:
        ```
        npm install -g @azure/static-web-apps-cli
        ```
