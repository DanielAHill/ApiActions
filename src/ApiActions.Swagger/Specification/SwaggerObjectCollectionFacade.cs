// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiActions.Swagger.Specification
{
    public class SwaggerObjectCollectionFacade<T> : ICollection<T>, ICustomSwaggerJsonSerializable
    {
        private readonly IList<T> _items;

        public int Count => _items.Count;
        public bool IsReadOnly => _items.IsReadOnly;

        public SwaggerObjectCollectionFacade()
        {
            _items = new List<T>();
        }

        public SwaggerObjectCollectionFacade(IEnumerable<T> items)
        {
            _items = items.ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public void SerializeJson(StringBuilder builder, Action<object, StringBuilder, int> serializeChild,
            int recursionsLeft)
        {
            var writeComma = false;

            builder.Append('{');
            foreach (var item in _items)
            {
                if (writeComma)
                {
                    builder.Append(',');
                }
                else
                {
                    writeComma = true;
                }

                serializeChild(item, builder, recursionsLeft);
            }

            builder.Append('}');
        }
    }
}