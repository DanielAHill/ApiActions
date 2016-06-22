﻿#region Copyright
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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions.AbstractModeling;
using DanielAHill.AspNet.ApiActions.Responses;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.Authorization
{
    public class AuthRoleAttribute : AuthAttribute
    {
        private readonly string[] _roles;
        private readonly int _failureStatusCode;

        public AuthRoleAttribute(params string[] roles) 
            : this((int)HttpStatusCode.Forbidden, roles)
        {
        }

        public AuthRoleAttribute(HttpStatusCode failureStatusCode, params string[] roles)
            : this((int)failureStatusCode, roles)
        {

        }

        public AuthRoleAttribute(int failureStatusCode, params string[] roles)
            : this(failureStatusCode, (int)HttpStatusCode.Unauthorized, roles)
        {
            
        }

        public AuthRoleAttribute(HttpStatusCode failureStatusCode, HttpStatusCode notAuthenticatedStatusCode, params string[] roles)
            :this((int)failureStatusCode, (int)notAuthenticatedStatusCode, roles)
        {
            
        }

        public AuthRoleAttribute(int failureStatusCode, int notAuthenticatedStatusCode, params string[] roles)
            :base(notAuthenticatedStatusCode)
        {
            if (roles == null) throw new ArgumentNullException(nameof(roles));
            _failureStatusCode = failureStatusCode;
            _roles = roles;
        }

        public override async Task<ApiActionResponse> AuthorizeAsync(HttpContext context, AbstractModel abstractModel, CancellationToken cancellationToken)
        {
            var result = await base.AuthorizeAsync(context, abstractModel, cancellationToken).ConfigureAwait(false);

            if (result == null && _roles.Any(r => !context.User.IsInRole(r)))
            {
                return new ObjectResponse(_failureStatusCode);
            }

            return result;
        }
    }
}