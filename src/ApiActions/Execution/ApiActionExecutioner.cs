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
using System.Threading;
using System.Threading.Tasks;
using ApiActions.Responses;

namespace ApiActions.Execution
{
    internal class ApiActionExecutioner : IApiActionExecutioner
    {
        private static readonly Func<IApiAction, CancellationToken, Task>[] PreExecutionTasks =
        {
            AuthorizeAsync, ValidateModelAsync, PreloadDataAsync, ValidateModelDataAsync, AuthorizeDataAsync
        };

        public async Task<ApiActionResponse> ExecuteAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            // ReSharper disable once ForCanBeConvertedToForeach - array indexing is faster
            for (var x = 0; x < PreExecutionTasks.Length; x++)
            {
                await PreExecutionTasks[x](apiAction, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
                if (apiAction.ActionResponse != null)
                {
                    return apiAction.ActionResponse;
                }
            }

            // ExecuteAsync
            var response = await apiAction.ExecuteAsync(cancellationToken) ?? apiAction.ActionResponse;
            cancellationToken.ThrowIfCancellationRequested();
            return response ?? apiAction.ActionResponse ?? NoContentResponse.Singleton;
        }

        protected static Task AuthorizeAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            return apiAction.AuthorizeAsync(cancellationToken);
        }

        protected static Task PreloadDataAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            return apiAction.PreloadDataAsync(cancellationToken);
        }

        protected static Task AuthorizeDataAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            return apiAction.AuthorizeDataAsync(cancellationToken);
        }

        private static Task ValidateModelDataAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            return apiAction.ValidateModelDataAsync(cancellationToken);
        }

        private static Task ValidateModelAsync(IApiAction apiAction, CancellationToken cancellationToken)
        {
            return apiAction.ValidateModelAsync(cancellationToken);
        }
    }
}