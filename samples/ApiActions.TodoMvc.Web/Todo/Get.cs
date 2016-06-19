using System;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions;
using ApiActions.TodoMvc.Domain;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Get]
    [Category("TodoMVC Backend Implementation")]
    public class Get : ApiAction
    {
        private readonly ITodoRepository _todoRepository;

        public Get(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, typeof(TodoEntry[]), "A list of Todos")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Response(200, _todoRepository.GetAll());
        }
    }
}