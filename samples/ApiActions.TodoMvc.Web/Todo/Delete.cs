using System;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Delete]
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