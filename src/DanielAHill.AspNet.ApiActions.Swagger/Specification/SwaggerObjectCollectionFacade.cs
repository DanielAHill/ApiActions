using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public class SwaggerObjectCollectionFacade<T> : ICollection<T>, ICustomSwaggerSerializable
    {
        private readonly IList<T> _items;

        public int Count { get { return _items.Count; } }
        public bool IsReadOnly { get { return _items.IsReadOnly; } }

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

        public void Serialize(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
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