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

using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    internal static class StringBuilderJsonExtensions
    {
        internal static void AppendJsonDelimited(this StringBuilder builder, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach - For loop is faster than foreach in this scenario
            for (var x = 0; x < value.Length; x++)
            {
                var c = value[x];

                switch (c)
                {
                    case '"':
                        builder.Append(@"\""");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '/':
                        builder.Append(@"\/");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case ' ':
                        builder.Append(c);
                        break;
                    default:
                        if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            builder.Append(@"\u" + ((int)c).ToString("X4"));
                        }
                        break;
                }
            }
        }
    }
}