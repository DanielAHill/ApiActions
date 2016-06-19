using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.TodoMvc.Domain;
using DanielAHill.AspNet.ApiActions;

namespace ApiActions.TodoMvc.Web.Todo
{
    [Patch]
    [UrlSuffix("{Id:int}")]
    public class PatchSingle : ApiAction<TodoEntry>
    {
        private readonly ITodoRepository _todoRepository;

        public PatchSingle(ITodoRepository todoRepository)
        {
            if (todoRepository == null) throw new ArgumentNullException(nameof(todoRepository));
            _todoRepository = todoRepository;
        }

        [Response(200, typeof(TodoEntry), "Todo item updated")]
        [Response(404, "Task not found")]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            // Get current task, by Id
            var current = _todoRepository.Get(Data.Id);

            if (current == null)
            {   // Task not found
                return Response(404);
            }

            // Get Cached Type Details which includes compiled property reader/writers.
            var todoTypeDetails = typeof (TodoEntry).GetTypeDetails();
            
            foreach (var propertyWriter in todoTypeDetails.PropertyWriters)
            {
                if ("Id".Equals(propertyWriter.Name, StringComparison.OrdinalIgnoreCase)
                    || "Url".Equals(propertyWriter.Name, StringComparison.OrdinalIgnoreCase))
                {   // Non editable properties
                    continue;
                }

                var propertyModel = AbstractModel.FirstOrDefault(m => m.Name.Equals(propertyWriter.Name, StringComparison.OrdinalIgnoreCase));

                if (propertyModel != null)
                {
                    // Get Value from request object, which has been parsed, validated, and correctly converted (could be null, which is okay)
                    var convertedProperty = todoTypeDetails.PropertyReaders.First(r => r.Name.Equals(propertyWriter.Name)).Read(Data);

                    // Save new value to correct property of current todo item
                    propertyWriter.Write(current, convertedProperty);
                }
            }

            // Save current Todo item
            _todoRepository.Save(current);

            // Retun Success Response
            return Response(200, current);
        }
    }
}