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
- The front-end client was written in [Angular](https://angular.io/) and [Typescript](https://www.typescriptlang.org/) and [Visual Studio Code IDE](https://code.visualstudio.com/).
- The back-end application was written in [C# 10](https://docs.microsoft.com/en-us/dotnet/csharp/) using [Dotnet 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and [Visual Studio 2022 IDE](https://visualstudio.microsoft.com/vs/).
- Hosting happens with [Azure Static Web App](https://azure.microsoft.com/en-us/services/app-service/static/).
