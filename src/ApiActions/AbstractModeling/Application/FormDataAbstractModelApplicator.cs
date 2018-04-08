﻿// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;

namespace ApiActions.AbstractModeling.Application
{
    public class FormDataAbstractModelApplicator : IAbstractModelApplicator
    {
        public string[] ContentTypes { get; } = {"application/x-www-form-urlencoded", "multipart/form-data"};

        public bool Handles(IAbstractModelApplicationRequestContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ContentType) ||
                !MediaTypeHeaderValue.TryParse(context.ContentType, out var contentType))
            {
                return false;
            }

            return contentType.MediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
                   || contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ApplyAsync(IAbstractModelApplicationRequestContext context, AbstractModel abstractModel,
            CancellationToken cancellationToken)
        {
            var formCollection = await new FormFeature(context.Form).ReadFormAsync(cancellationToken);

            foreach (var key in formCollection.Keys)
            {
                abstractModel.Add(new AbstractModel(key, formCollection[key]));
            }

            foreach (var file in formCollection.Files)
            {
                if (ContentDispositionHeaderValue.TryParse(file.ContentDisposition, out var contentDispositionHeaderValue))
                {
                    abstractModel.Add(new AbstractModel(contentDispositionHeaderValue.Name.ToString(), file));
                }
            }
        }
    }
}