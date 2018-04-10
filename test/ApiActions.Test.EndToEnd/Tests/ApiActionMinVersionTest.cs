// Copyright (c) 2018-2018 Daniel A Hill. All rights reserved.
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiActions.Test.EndToEnd.Tests
{
    [TestClass]
    public class ApiActionMinVersionTest : ApiActionsEndToEndTest
    {
        [TestMethod]
        public void SameVersionMatches()
        {
            var app = CreateApp(
                s => { s.AddApiActions(this.GetType().Assembly); },
                a => { a.UseApiActions(); });

            var context = app.Execute(new HttpRequestFeature
            {
                Method = "GET",
                Path = "/ApiActions/Versioned/MinVersion",
                QueryString = "version=2.0"
            });

            Write(context.Response);

            Assert.AreEqual(200, context.Response.StatusCode);
        }

        [TestMethod]
        public void GreaterVersionMatches()
        {
            var app = CreateApp(
                s => { s.AddApiActions(this.GetType().Assembly); },
                a => { a.UseApiActions(); });

            var context = app.Execute(new HttpRequestFeature
            {
                Method = "GET",
                Path = "/ApiActions/Versioned/MinVersion",
                QueryString = "version=2.1"
            });

            Write(context.Response);

            Assert.AreEqual(200, context.Response.StatusCode);
        }

        [TestMethod]
        public void LesserVersionDoesNotMatch()
        {
            var app = CreateApp(
                s => { s.AddApiActions(this.GetType().Assembly); },
                a => { a.UseApiActions(); });

            var context = app.Execute(new HttpRequestFeature
            {
                Method = "GET",
                Path = "/ApiActions/Versioned/MinVersion",
                QueryString = "version=1.1"
            });

            Write(context.Response);

            Assert.AreEqual(404, context.Response.StatusCode);
        }

        [TestMethod]
        public void UnversionedMatches()
        {
            var app = CreateApp(
                s => { s.AddApiActions(this.GetType().Assembly); },
                a => { a.UseApiActions(); });

            var context =
                app.Execute(new HttpRequestFeature {Method = "GET", Path = "/ApiActions/Versioned/MinVersion"});

            Write(context.Response);

            Assert.AreEqual(200, context.Response.StatusCode);
        }
    }
}