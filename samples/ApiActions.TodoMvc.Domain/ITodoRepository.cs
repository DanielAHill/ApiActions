using System.Collections.Generic;

namespace ApiActions.TodoMvc.Domain
{
    public interface ITodoRepository
    {
        IReadOnlyCollection<TodoEntry> GetAll();
        TodoEntry Get(int id);
        TodoEntry Save(TodoEntry entry);
        void Delete(int id);
    }
}