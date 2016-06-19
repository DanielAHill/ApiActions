using System;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Post]
    public class Post : ApiAction<TodoEntry>
    {
        private readonly ITodoRepository _todoRepository;

        public Post(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, typeof(TodoEntry), "Todo item saved")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            var savedData = _todoRepository.Save(Data);

            if (savedData.Url == null)
            {
                // Was created, update url
                savedData.Url = string.Concat(HttpRequest.Scheme, "://", HttpRequest.Host, HttpRequest.Path, "/", Data.Id);
                _todoRepository.Save(savedData);
            }

            return Response(200, savedData);
        }
    }
}