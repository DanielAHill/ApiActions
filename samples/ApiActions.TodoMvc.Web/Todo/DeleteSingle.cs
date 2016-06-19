using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Delete]
    [UrlSuffix("{Id:int}")]
    [Category("TodoMVC Backend Implementation")]
    public class DeleteSingle : ApiAction<DeleteSingle.Request>
    {
        private readonly ITodoRepository _todoRepository;

        public DeleteSingle(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, "Todo Item Successfully Deleted")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            _todoRepository.Delete(Data.Id);
            return Response(200);
        }

        public class Request
        {
            [Required]
            public int Id { get; set; }
        }
    }
}