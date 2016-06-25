#region Copyright
// Copyright (c) 2016 Daniel A Hill. All rights reserved.
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
        public string Description { get; set; }
        public string TermsOfService { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactUrl { get; set; }

        public string LicenseName { get; set; }
        public string LicenseUrl { get; set; }

        #endregion

        public string ExternalDocumentationDescription { get; set; }
        public string ExternalDocumentationUrl { get; set; }

        public string[] DefaultMethods { get; set; } = new string[] { "GET" };
    }
}