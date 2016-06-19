using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Get]
    [UrlSuffix("{Id:int}")]
    public class GetSingle : ApiAction<GetSingle.Request>
    {
        private readonly ITodoRepository _todoRepository;

        public GetSingle(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, typeof(TodoEntry), "Requested Todo")]
        [Response(404, "Requested Todo not found")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            var todoEntry = _todoRepository.Get(Data.Id);

            if (todoEntry == null)
            {
                return Response(404);
            }

            return Response(200, todoEntry);
        }

        public class Request
        {
            [Required]
            public int Id { get; set; }
        }
    }
}