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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions;

namespace ApiActionsSandbox.Web.Header
{
    [Summary("All Headers")]
    [Description("Returns all HTTP headers provided by client request")]
    [HttpMethod("GET", "POST")]
    [Category("Header Insights")]
    public class All : ApiAction
    {
        [Response(200, typeof(Dictionary<string, string>), "Dictionary of request headers")] 
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
