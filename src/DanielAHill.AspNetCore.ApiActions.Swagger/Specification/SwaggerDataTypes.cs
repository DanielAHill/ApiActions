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
namespace DanielAHill.AspNetCore.ApiActions.Swagger.Specification
{
    /// <summary>
    /// Swagger 2.0 Data Types
    /// </summary>
    /// <remarks>Refer to Swagger RESTful API Documentation Specification License for license info</remarks>
    public enum SwaggerDataTypes
    {
        /// <summary>
        /// Int32 (signed 32 bits)
        /// </summary>
        Integer = 0,

        /// <summary>
        /// Int64 (signed 64 bits)
        /// </summary>
        Long = 1,

        /// <summary>
        /// Float
        /// </summary>
        Float = 2,

        /// <summary>
        /// Double
        /// </summary>
        Double = 3,

        /// <summary>
        /// String
        /// </summary>
        String = 4,

        /// <summary>
        /// Byte or Byte Array -> Serialized as base64 encoded characters
        /// </summary>
        Byte = 5,

        /// <summary>
        /// Byte or Byte Array -> Serialized as octet string
        /// </summary>
        Binary = 6,

        /// <summary>
        /// Boolean - true/false
        /// </summary>
        Boolean =  7,

        /// <summary>
        /// RFC3339 - <c>full-date</c>
        /// </summary>
        Date = 8,

        /// <summary>
        /// RFC3339 - <c>date-time</c>
        /// </summary>
        DateTime = 9,

        /// <summary>
        /// String - Used to hint UIs the input needs to be obscured.
        /// </summary>
        Password = 10
    }
}
