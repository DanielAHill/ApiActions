// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.AbstractModeling;
using ApiActions.AbstractModeling.Application;

namespace ApiActions.Serialization
{
    public class EdgeDeserializer : IEdgeDeserializer
    {
        private readonly IAbstractModelApplicator[] _abstractModelApplicators;

        public EdgeDeserializer(IEnumerable<IAbstractModelApplicator> abstractModelApplicators)
        {
            if (abstractModelApplicators == null) throw new ArgumentNullException(nameof(abstractModelApplicators));
            _abstractModelApplicators = abstractModelApplicators.ToArray();
        }

        public async Task<AbstractModel> DeserializeAsync(IAbstractModelApplicationRequestContext context,
            CancellationToken cancellationToken)
        {
            var abstractModel = new AbstractModel();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var x = 0; x < _abstractModelApplicators.Length; x++)
            {
                var applicator = _abstractModelApplicators[x];
                if (applicator.Handles(context))
                {
                    await applicator.ApplyAsync(context, abstractModel, cancellationToken);
                }
            }

            return abstractModel;
        }
    }
}