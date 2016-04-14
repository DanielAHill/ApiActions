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
using DanielAHill.AspNet.ApiActions.AbstractModeling;
using DanielAHill.AspNet.ApiActions.Responses;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.Authorization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthAttribute : Attribute, IAuthFilter
    {
        private readonly int _failureStatusCode;

        public AuthAttribute()
            :this(HttpStatusCode.Unauthorized)
        {
            
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public AuthAttribute(HttpStatusCode failureStatusCode)
            :this((int)failureStatusCode)
        {
            
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public AuthAttribute(int failureStatusCode)
        {
            _failureStatusCode = failureStatusCode;
        }

        public virtual Task<ApiActionResponse> AuthorizeAsync(HttpContext context, AbstractModel abstractModel, CancellationToken cancellationToken)
        {
            return Task.FromResult<ApiActionResponse>(!context.User.Identity.IsAuthenticated ? new ObjectResponse(_failureStatusCode) : null);
        }
    }
}