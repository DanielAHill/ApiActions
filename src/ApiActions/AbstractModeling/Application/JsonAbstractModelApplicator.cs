// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ApiActions.AbstractModeling.Application
{
    public class JsonAbstractModelApplicator : IAbstractModelApplicator
    {
        public string[] ContentTypes { get; } = {"application/json"};

        public bool Handles(IAbstractModelApplicationRequestContext context)
        {
            return context.ContentType != null &&
                   context.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        }

        private static readonly char[] ObjectStartChar = {'{'};

        public async Task ApplyAsync(IAbstractModelApplicationRequestContext context, AbstractModel abstractModel,
            CancellationToken cancellationToken)
        {
            var reader = new SectionedStreamReader(context.Stream, cancellationToken);

            var section = await reader.ReadSection(ObjectStartChar);

            if (!string.IsNullOrEmpty(section))
            {
                throw new InvalidDataException("Expected start of '{'");
            }

            if (!reader.EndOfStream)
            {
                await ApplyClassAsync(reader, abstractModel);
            }
        }

        private static async Task ApplyClassAsync(SectionedStreamReader reader, AbstractModel abstractModel)
        {
            do
            {
                await ApplyPropertyAsync(reader, abstractModel);
            } while (!reader.EndOfStream && reader.LastDelimiter != '}');
        }

        private static readonly char[] PostPropertyNameChars = {'"', ':', '}', ','};
        private static readonly char[] Quote = {'"'};
        private static readonly char[] Colon = {':'};
        private static readonly char[] ValueChars = {'"', ',', '[', ']', '{', '}'};

        private static async Task ApplyPropertyAsync(SectionedStreamReader reader, AbstractModel abstractModel)
        {
            // Read Property Name
            var name = await reader.ReadSection(PostPropertyNameChars);

            if (reader.EndOfStream && string.IsNullOrEmpty(name))
            {
                // Natural end of stream
                return;
            }

            switch (reader.LastDelimiter)
            {
                case '}':
                    // End of object detected
                    return;
                case ',':
                    // Left-over (or empty) comma from previous property
                    if (!string.IsNullOrEmpty(name))
                    {
                        throw new InvalidDataException($"Expected ':' but got '${name},'");
                    }

                    return;
                case '"' when string.IsNullOrEmpty(name):
                    name = await reader.ReadSection(Quote, false);
                    await reader.ReadSection(Colon);
                    break;
            }

            if (reader.LastDelimiter != ':')
            {
                throw new InvalidDataException($"Expected ':' but got '{reader.LastDelimiter}'");
            }

            var abstractProperty = new AbstractModel(name);

            // Read Value
            await ApplyValue(reader, abstractProperty, false);

            abstractModel.Add(abstractProperty);
        }

        private static async Task ApplyValue(SectionedStreamReader reader, AbstractModel abstractModel, bool isArray)
        {
            var value = await reader.ReadSection(ValueChars);

            switch (reader.LastDelimiter)
            {
                case '"':
                    value = await reader.ReadSection(Quote, false);
                    break;
                case '[':
                    do
                    {
                        await ApplyValue(reader, abstractModel, true);
                    } while (!reader.EndOfStream && reader.LastDelimiter != ']');

                    break;
                case '{':
                    var workingModel = abstractModel;

                    if (isArray)
                    {
                        workingModel = new AbstractModel();
                        abstractModel.AddValue(workingModel);
                    }

                    await ApplyClassAsync(reader, workingModel);
                    break;
            }

            if (!string.IsNullOrEmpty(value))
            {
                abstractModel.AddValue(value);
            }
        }
    }
}