﻿// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

// ReSharper disable CheckNamespace

namespace ApiActions.AbstractModeling.Application
{
    public interface IAbstractModelApplicationRequestContext
    {
        //IServiceProvider ApplicationServices { get; }
        IServiceProvider RequestServices { get; }
        IDictionary<object, object> Items { get; }

        RouteData RouteData { get; }
        ClaimsPrincipal User { get; }
        ConnectionInfo Connection { get; }
        IFeatureCollection Features { get; }

        IRequestCookieCollection Cookies { get; }
        string ContentType { get; }
        IFormCollection Form { get; }

        string TraceIdentifier { get; }
        IQueryCollection Query { get; }
        QueryString QueryString { get; }

        Stream Stream { get; }
    }
}