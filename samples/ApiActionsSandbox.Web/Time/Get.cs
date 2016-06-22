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
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions;

namespace ApiActionsSandbox.Web.Time
{
    [Get]
    [Summary("Current Time")]
    [Description("Gets current local and UTC server time")]
    public class Get : ApiAction
    {
        [Response(401, Description = "1/100 chance of returning unauthorized, you are unlucky")]
        public override Task AuthorizeAsync(CancellationToken cancellationToken)
        {
            if (Guid.NewGuid().GetHashCode() % 100 == 0)
            {
                return Response(HttpStatusCode.Unauthorized);
            }

            return Task.FromResult(true);
        }

        [Response(200, Type = typeof(ResponseModel))]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Response(new ResponseModel { Utc = DateTime.UtcNow, Local = DateTime.Now});
        }

        private class ResponseModel
        {
            public DateTime Utc { get; set; }
            public DateTime Local { get; set; }
        }
    }
}