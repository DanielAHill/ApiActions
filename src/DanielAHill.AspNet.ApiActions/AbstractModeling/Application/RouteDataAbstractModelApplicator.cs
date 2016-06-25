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

using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNet.ApiActions.AbstractModeling.Application
{
    public class RouteDataAbstractModelApplicator : IAbstractModelApplicator
    {
        public string[] ContentTypes { get; } = null;

        public bool Handles(IAbstractModelApplicationRequestContext context)
        {
            return true;
        }

        public Task ApplyAsync(IAbstractModelApplicationRequestContext context, AbstractModel abstractModel, CancellationToken cancellationToken)
        {
            foreach (var parameter in context.RouteData.Values)
            {
                abstractModel.Add(new AbstractModel(parameter.Key, parameter.Value));
            }

            return Task.FromResult(true);
        }
    }
}