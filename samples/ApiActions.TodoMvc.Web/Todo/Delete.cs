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
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Delete]
    [Category("TodoMVC Backend Implementation")]
    public class Delete : ApiAction
    {
        private readonly ITodoRepository _todoRepository;

        public Delete(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, "All Todo Items Deleted")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var todo in _todoRepository.GetAll())
            {
                _todoRepository.Delete(todo.Id);   
            }
            return Response(200);
        }
    }
}