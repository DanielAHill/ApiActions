using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiActions.TodoMvc.Domain
{
    public class RamTodoRepository : ITodoRepository
    {
        private readonly IDictionary<int, TodoEntry> _entries = new Dictionary<int, TodoEntry>();
        private int _currentIndex = 0;

        public IReadOnlyCollection<TodoEntry> GetAll()
        {
            lock (_entries)
            {
                return _entries.Values.OrderBy(v => v.Order).ToList();
            }
        }

        public TodoEntry Get(int id)
        {
            lock (_entries)
            {
                TodoEntry entry;
                _entries.TryGetValue(id, out entry);
                return entry;
            }
        }

        public TodoEntry Save(TodoEntry entry)
        {
            lock (_entries)
            {
                if (entry.Id == default(int))
                {
                    entry.Id = ++_currentIndex;
                }

                _entries[entry.Id] = entry;
            }

            return entry;
        }

        public void Delete(int id)
        {
            lock (_entries)
            {
                _entries.Remove(id);
            }
        }
    }
}
