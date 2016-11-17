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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace DanielAHill.AspNetCore.ApiActions.Responses
{
    public class InvalidModelResponse : ObjectResponse
    {
        public InvalidModelResponse(params ValidationResult[] modelErrors)
            :this((IEnumerable<ValidationResult>)modelErrors)
        { }

        public InvalidModelResponse(IEnumerable<ValidationResult> modelErrors)
            :this((int)HttpStatusCode.BadRequest, modelErrors)
        {
            
        }

        public InvalidModelResponse(HttpStatusCode statusCode, IEnumerable<ValidationResult> modelErrors)
            : this((int)statusCode, modelErrors)
        {

        }

        public InvalidModelResponse(int statusCode, IEnumerable<ValidationResult> modelErrors)
            : base(statusCode, Convert(modelErrors))
        {

        }

        private static BadRequestDetails Convert(IEnumerable<ValidationResult> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            var convertDictionary = new Dictionary<string, PropertyValidationResult>();

            var globalErrors = new List<string>();

            foreach (var error in errors)
            {
                var hasPropertyErrors = false;
                foreach (var memberName in error.MemberNames)
                {
                    Add(memberName, error.ErrorMessage, convertDictionary);
                    hasPropertyErrors = true;
                }

                if (!hasPropertyErrors)
                {
                    globalErrors.Add(error.ErrorMessage);
                }
            }

            return new BadRequestDetails()
            {
                Errors = globalErrors.Any() ? globalErrors : null,
                Inputs = convertDictionary.Values.Any() ? convertDictionary.Values.ToList() : null
            };
        }

        private static void Add(string memberName, string errorMessage, IDictionary<string, PropertyValidationResult> convertDictionary)
        {
            PropertyValidationResult validationResult;
            if (!convertDictionary.TryGetValue(memberName, out validationResult))
            {
                validationResult = new PropertyValidationResult(memberName);
                convertDictionary.Add(memberName, validationResult);
            }

            validationResult.Errors.Add(errorMessage);
        }
    }
}