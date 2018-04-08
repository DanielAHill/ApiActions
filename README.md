#ApiActions
API Middleware for AspNet with emphasis on REST-ful capabilities, versioning, performance, and extensibility.

## Build Status
| Master | Beta | Alpha |
|--------|------|-------|
| ![Master Build Status](https://danielahill.visualstudio.com/DefaultCollection/_apis/public/build/definitions/b6ac9a5b-dc10-4a59-8ceb-2cdf608455d1/7/badge) | ![Beta Build Status](https://danielahill.visualstudio.com/DefaultCollection/_apis/public/build/definitions/b6ac9a5b-dc10-4a59-8ceb-2cdf608455d1/8/badge) | ![Alpha Build Status](https://danielahill.visualstudio.com/DefaultCollection/_apis/public/build/definitions/b6ac9a5b-dc10-4a59-8ceb-2cdf608455d1/6/badge)|

## Feature Overview
 - Ability to version API for continuous delivery and/or rolling versions
 - Easily support RESTful API contracts
 - Encourage concise, organized, easy to find/read code
 - Encourage enhanced security implementations
 - Supports both .NET 4.51+ and DNX/ASP.NET Core 1.0

## Installation
Installing API Actions to your Visual Studio project via NuGet (hosted on [NuGet.org](https://www.nuget.org/)) is easy. You can search for `ApiActions` in **NuGet Package Manager** or type the following in the **NuGet Command Window**:

    nuget install ApiActions

## Initializing WebActions
In order for ApiActions to function properly, registration steps are required for both ASP.NET's dependency injection infrastructure as well as an OWIN middleware.

All initialization takes place in the web project's `Startup.cs` class.

### Registering Api Actions in Dependency Injection
The API Actions middleware resolves both ApiAction implementations from the ASP.NET's dependency injection infrastructure. The following `Startup.cs` code registers all of the ApiAction implementations with DI. API Actions from multiple assemblies can be registered, but at one registration is required.

    public void ConfigureServices(IServiceCollection services)
    {
        // Add Api actions for web assembly
        services.AddWebActions(typeof (Startup).GetTypeInfo().Assembly);
    }

### Registering the API Actions Middleware
The following code registered the API Actions middleware in the `Startup.cs` class and can be registered along side other popular handlers such as *Static Files*, *MVC*, or even *Web API*.

    public void Configure(IApplicationBuilder app)
    {
        // Register APi Action Middleware
        app.UseApiActions();
    }
    
### Creating your First API Action
Create a folder called `Hello` at the base of your web project. Inside this folder, create a new class called `SayHello` with the following code, replacing *(YourWebProjectName)* with the name of your web project:

    using System.Threading;
    using System.Threading.Tasks;
    using DanielAHill.AspNetCore.ApiActions;

    [HttpGet]
    namespace (YourWebProjectName).Hello
    {
        public class SayHello : ApiAction
        {
            public override async Task<IApiActionResponse> ExecuteAsync(
                        CancellationToken cancellationToken)
            {
                return Respond("Hello Web Actions!");
            }
        }
    }
    
Run the web project and navigate to **/hello** using your web browser. That's it. You've created your first, albiet very simple, API Action!

The url in the example above is specified by the namespace. The class name irrelevant for url routing purposes but can be very helpful to developers when creating different API Actions for different HTTP verbs (use HttpPost, HttpPut, etc). Change the namespace to `(YourProjectName).Api.Hello` and the new url will be **/api/hello**.

### Accepting Data
Create a folder called `Reply` at the base of your web project. Inside this folder, create a new class called `ReplyWithTime` with the following code, replacing *(YourWebProjectName)* with the name of your web project.

    using System.Threading;
    using System.Threading.Tasks;
    using DanielAHill.AspNetCore.ApiActions;

    namespace (YourWebProjectName).Reply
    {
        public class ReplyWithTime : ApiAction<ReplyWithTime.Request>
        {
            public override async Task<IApiActionResponse> ExecuteAsync(
                        CancellationToken cancellationToken)
            {
                return Respond(new {
                        Text = Model.Text,
                        Time = DateTime.Now
                    });
            }
            
            public class Request
            {
                [Required]
                [MinLength(2)]
                public string Text { get; set; }
            }
        }
    }

Run the web project and navigate to **/reply?text=Hello** using your web browser. You'll see your response echoed back to you along with the server time. You can also send json, xml, url encoded form, or multi-part form to the same url and receive the same results. In future sections, you will learn how to embed parameters into the url and configure custom deserializers to support additional content types.

It is important to note a few differences between the `SayHello` example and the `ReplyWithTime` example above. The fist significant difference is the nested Request class. This is the request model for the API Action and has Data Attributes in order to enforce input requirements/restrictions. Submitting an invalid model will result in a 400 - Bad Request response, that includes the field name and a human readible message describing the validation concern. When model validation fails, the Execute method is never called. Most WebAction developers tend to name the request model *Request* and nest it in the associated API Action in order to preserve a 1-to-1 association between request model and API Action. However, developers are free to place and name the request model class wherever they desire.

The second significant difference is the lack of the `HttpGet` attribute. This route filter was removed so the web action can accept all HTTP verbs, allowing you to test out the various ways in which API Actions automatically pulls data into the request model (url parameters, json, xml, and more).

### Learning More
The API Actions library is a very robust, extensible, and powerful framework. It gives developers complete control over url routes, data inputs, input/output serialization, authorization, versioning, and more. The full, updated, documentation can be found online at https://danielahill.gitbooks.io/apiactions/, including a downloadable PDF and formats for common e-book readers.

## Contributing
Contributing to this project is encouraged. To contribute, send a pull request to the development branch containing your code an other artifacts. Before your pull request can be approved, you will need to sign and deliver the Contributor Agreement below to the e-mail specified in the agreement:

* [Individual Contributor Agreement](https://raw.githubusercontent.com/DanielAHill/ApiActions/develop/_docs/individual-contributor-license-agreement-1.1-2017-02-08-18_12_06.pdf)
* [Entity Contributor Agreement](https://raw.githubusercontent.com/DanielAHill/ApiActions/develop/_docs/entity-contributor-license-agreement-1.1-2017-02-08-18_12_08.pdf)


## License
Copyright (c)2015-2016 Daniel A Hill. All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

[http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.