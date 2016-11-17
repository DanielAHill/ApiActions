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

using System;
using DanielAHill.AspNetCore.ApiActions.Swagger.Specification;

namespace DanielAHill.AspNetCore.ApiActions.Swagger.Creation
{
    public class SwaggerTypeProvider : ISwaggerTypeProvider
    {
        public virtual SwaggerType GetSwaggerType(Type type)
        {
            var typeDetails = type.GetTypeDetails();

            if (typeDetails.IsNumeric)
            {
                if (type == typeof (double)
                    || type == typeof (double?)
                    || type == typeof (float)
                    || type == typeof (float?)
                    || type == typeof (decimal)
                    || type == typeof (decimal?)
                    )
                {
                    return SwaggerType.Number;
                }

                return SwaggerType.Integer;
            }

            if (type == typeof (bool) || type == typeof (bool?))
            {
                return SwaggerType.Boolean;
            }

            if (typeDetails.IsCollection)
            {
                return SwaggerType.Array;
            }

            if (typeDetails.IsValue)
            {
                return SwaggerType.String;
            }

            return SwaggerType.Object;
        }

        public Type GetTypeToDocument(Type type)
        {
            var typeDetails = type.GetTypeDetails();

            if (!typeDetails.IsCollection)
            {
                return type;
            }

            var elementType = type.GetElementType();
            if (elementType == null && type.GenericTypeArguments != null && type.GenericTypeArguments.Length == 1)
            {
                elementType = type.GenericTypeArguments[0];
            }

            return elementType ?? type;
        }
    }
}