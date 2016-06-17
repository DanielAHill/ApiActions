#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
namespace DanielAHill.AspNet.ApiActions.Swagger
{
    public class SwaggerOptions
    {
        #region Routing
        public string RequestUrl { get; set; } = "swagger.json";
        public string ApiRoutePrefix { get; set; } = null;
        public string HostName { get; set; } = null;
        public bool IncludeVersionsAsExternalDocuments { get; set; } = true;
        #endregion

        #region Swagger Info
        public string Title { get; set; }
        public string Description { get; set; } = "This is a description of the API";
        public string TermsOfService { get; set; } = "This is the Terms of Service";

        public string ContactName { get; set; } = "Contact Name";
        public string ContactEmail { get; set; } = "insert@e.mail";
        public string ContactUrl { get; set; } = "http://www.google.com/webhp?q=contact+info";

        public string LicenseName { get; set; } = "Unknown License";
        public string LicenseUrl { get; set; } = "https://www.google.com/webhp?q=unknown+license";

        #endregion

        public string ExternalDocumentationDescription { get; set; }
        public string ExternalDocumentationUrl { get; set; }

        public string[] DefaultMethods { get; set; } = new string[] { "GET" };
    }
}