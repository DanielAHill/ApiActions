#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
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
