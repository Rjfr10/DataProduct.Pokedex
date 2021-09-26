## Pre-requisites:
 - Visual Studio 2019 >= 16.8 (or alternate IDE)
 - Use Preview Feature
 - .Net 5 SDK Installed for appropriate system (64\32bit as required)
 - docker cli
 - Docker Desktop (recommeded - if you want to run the docker.compose file locally) 

## How to Run: 
- In Visual Studio:
If you have visual studio installed but do not have docker, then you can set the "DataProduct.Pokedex.Api" as the launching application by right clicking on the project 
Once you have done this, may simply click on the green "Debug" (IIS Express) on the top centre of your screen (usually) 
If you have docker, you can leave the setup of "docker-compose" as the Start up ( if you have changed it in the previous step, can switch back using the same process ) f local, Lgs are written to : c:\temp
- In Docker
docker build DataProduct.Pokedex.Api
docker-compose up
Should not be able to launch the url (swagger page) by navigating to the url and adding /swagger i.e: http://localhost:58241/swagger/ This is if you want to go through and test each endpoint from a simple ui.
			
## Overview:
For the purpose of this project and the basic requirements detailed in the specifications, the following decisions were made.
- Include all required functions within the same codebase and applications.
- Relied on the memory cache to support the desired load.
- Accept that the external systems will be available as much as this requires.

## Discussion Points:
Should this be going into production and required a level high availability and additional levels of resillience, one or more following improvements could be evaluated and implemented where applicable:
- Store the results of information retrieved by one or more of these providers for complete control of the information being served thus removing the need for these systems to be highly available.
- This could also help us remove the need to retrieve as much data that is currently being served by the external providers.
- Migrate the "importation"/retrival of the data to use a seperate system/function/framework ( i.e. Azure Function, Hangfire or seperate service app )
- At the moment, the data is retrieved by the external provider when the first call is made to the api and then cached thereafter...This could be eliminated if the data was fetched upfront and shared to each instance.
- This could also further reduce the amount of calls made thus potentially saving costs should we pay per call?
- Relianct on the external provider to always be available. Acknowledge this is mitigated with the caching mechanism however if a recycle occurs and the provider is not able to serve new data, then we are also down. 
- Make use of a distributed cache rather than an in-memory cache - There is no need to "fetch" the data from each instance if we going to have 3.
- The caching implemented is in memory hence, should this be load balanced, each instance will be making the same calls to the external provider thus potentially costing more than required if we were to pay per call...

## Improvement Considerations (Production Level Requirements):
- Assess Items mentioned above and determine way forward regarding the above.
- Reassess code for areas of improvement (Code Review will also likely have some changes)
  - Global Exception Handler.
  - Redact the information being sent out
	Include more specific detail where applicable.
	Add more detail to swagger docs ( include other types of errors etc. )
	Refine the Nlog to match production level requirements.
	Make use of structured logging for appropriate values.
  - Potentially look a decoupling external library from internal code if possible.
  - Implement a pretty start page for successful initialisation of api for when you launch the main url.
  - Spend some time cleaning up the existing unit tests and adding more code coverage.
  - Currently limitted to the "end-end" requirements but could focus on more of the specific code portions (unit tests)
  	- Setup Tests to run in docker container
  - Migrate certain test to integration level of tests. ie. The existing provider error check Test_GivenInValidPokemonName_ShouldReturnFailure is merely checking how our code behaves but in the event that this changes, then this test will still pass...
  - In addition to this, I also still need to verify beyond a doubt that this will fail in both scenarios...
	- Spend time adding negative test cases and checks for toggles
	- Add Integration Tests using a framework like Cypress (known)
  - Needs a nice red pokedex UI now...
  - Look at some form of health check between this application and the other systems.
	- This would allow a better mechanism to determine if the values "Not found" are due to bad data input or system being unavailable...
  - Add Authorization Headers ( UserTokens etc. )
  - Section for different types of diagonositic codes.
  - Implement Metrics to track total throughput.
  - Add Monitoring to the application
  - "Feature Flags" - nice to have to avoid the need to update the settings file when changes occur
  - Load Testing to be performed. ( Limited tp 5 calls a day due to free license )
  - Add Benchmarks Project for performance valuation
  - Security Testing
  - Build and Release Pipeline to be setup ( CI/CD )
  - Code Review to be completed by team
  - Documentation to be completed and reviewed

## Additional Information
- Additional fields are supplied in the response to determine if the value actually has been translated or not and what translation was used...
- In the event of the translation service not working, the ability to toggle this feature off to allow the response to succeed with a basic response is available
- Toggle for turning on/off the condition for translating "legendary" pokemon.
- Ability to change language for translated text where applicable from settings
- IOptionsMonitor used for "Variables" to allow application to remain running and take changes to configuration without the need to perform a recycle
- NLog Added for logging
- Swagger added for documentation
	
## Lessons Learnt:
 - Read the documentation more thoroughly for the client libraries prior to development start. 
   - Implemented my own caching to then learn that there is a level of caching included already.
 - The free version only allows 5 calls per hour before given you this, should have determined this ahead of time.
```cs
{
  "error": {
    "code": 429,
    "message": "Too Many Requests: Rate limit of 5 requests per hour exceeded. Please wait for 38 minutes and 36 seconds."
  }
}
```
