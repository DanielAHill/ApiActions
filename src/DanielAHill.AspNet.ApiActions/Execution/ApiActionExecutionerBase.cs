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
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions.Responses;

namespace DanielAHill.AspNet.ApiActions.Execution
{
    internal abstract class ApiActionExecutionerBase<T>
        where T : IApiAction
    {
        private readonly Func<T, CancellationToken, Task>[] _preExecutionTasks;

        protected ApiActionExecutionerBase(params Func<T, CancellationToken, Task>[] preExecutionTasks)
        {
#if DEBUG
            if (preExecutionTasks == null) throw new ArgumentNullException(nameof(preExecutionTasks));
            if (preExecutionTasks.Length == 0) throw new ArgumentException("Argument is empty collection", nameof(preExecutionTasks));
#endif
            _preExecutionTasks = preExecutionTasks;
        }

        protected static Task AuthorizeAsync(T apiAction, CancellationToken cancellationToken)
        {
            return apiAction.AuthorizeAsync(cancellationToken);
        }

        protected static Task PreloadDataAsync(T apiAction, CancellationToken cancellationToken)
        {
            return apiAction.PreloadDataAsync(cancellationToken);
        }

        protected static Task AuthorizeDataAsync(T apiAction, CancellationToken cancellationToken)
        {
            return apiAction.AuthorizeDataAsync(cancellationToken);
        }

        public async Task<ApiActionResponse> ExecuteAsync(T apiAction, CancellationToken cancellationToken)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var x = 0; x < _preExecutionTasks.Length; x++)
            {
                await _preExecutionTasks[x](apiAction, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
                if (apiAction.ActionResponse != null)
                {
                    return apiAction.ActionResponse;
                }
            }

            // ExecuteAsync
            var response = await apiAction.ExecuteAsync(cancellationToken).ConfigureAwait(false) ?? apiAction.ActionResponse;
            cancellationToken.ThrowIfCancellationRequested();
            return response ?? apiAction.ActionResponse ?? NoContentResponse.Singleton;
        }
    }
}