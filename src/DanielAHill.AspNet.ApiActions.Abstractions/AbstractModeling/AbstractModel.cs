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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DanielAHill.AspNet.ApiActions.AbstractModeling
{
    public class AbstractModel: IEnumerable<AbstractModel>
    {
        private IDictionary<string, AbstractModel> _children;
        private List<object> _values;

        private IDictionary<string, AbstractModel> Children {  get { return _children ?? (_children = new Dictionary<string, AbstractModel>());} }
        private IList<object> ValueList { get { return _values ?? (_values = new List<object>()); } }

        public string Name { get; }
        public bool IsEmpty { get { return ChildCount == 0 && ValueCount == 0; } }

        public IReadOnlyList<object> Values { get { return _values ?? (_values = new List<object>()); } }
        public int ValueCount { get { return _values?.Count ?? 0; } }
        //public object FirstValue {  get { return ValueCount == 0 ? null : Values[0]; } }

        public int ChildCount { get { return _children?.Count ?? 0; } }

        public AbstractModel this[string propertyName]
        {
            get
            {
                AbstractModel property;
                Children.TryGetValue(propertyName.ToLowerInvariant(), out property);
                return property;
            }
        }

        public AbstractModel()
        {
        }

        public AbstractModel(string name)
            :this(name, null)
        {
        }

        public AbstractModel(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name.ToLowerInvariant();

            if (value != null)
            {
                AddValue(value);
            }
        }

        public void Add(AbstractModel child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            AbstractModel existingChild;
            if (Children.TryGetValue(child.Name, out existingChild))
            {   // Merge
                if (existingChild.ChildCount > 0)
                {
                    existingChild.AddRange(child);
                }

                var childValueList = child.ValueList;
                var existingChildValueList = existingChild.ValueList;
                for (var x = 0; x < childValueList.Count; x++)
                {
                    var value = childValueList[x];
                    if (!existingChildValueList.Contains(value))
                    {
                        existingChildValueList.Add(value);
                    }
                }
            }
            else
            {   // Add
                Children.Add(child.Name, child);
            }
        }

        public void AddRange(IEnumerable<AbstractModel> children)
        {
            foreach (var child in children)
            {
                Add(child);
            }
        }

        public void Add(string childName, object childValue)
        {
            Add(new AbstractModel(childName, childValue));
        }

        public void AddValue(AbstractModel value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            ValueList.Add(value);
        }

        public void AddValue(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.GetType() != typeof(AbstractModel) && !value.IsAbstractModelValueType())
            {   
                throw new ArgumentException("Must be an abstract model value type", nameof(value));
            }

            ValueList.Add(value);
        }

        public IEnumerator<AbstractModel> GetEnumerator()
        {
            return Children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (_children != null && _children.Count > 0)
            {
                var values = _children.Values.Select(v => v.ToString()).Aggregate((a, b) => string.Concat(a, ", ", b));
                return Name == null ? string.Concat("{ ", values, "}") : string.Concat(Name, ": { ", values , " }");
            }

            if (_values != null && _values.Count > 1)
            {
                return string.Concat(Name, ": ", _values.Select(v => v?.ToString()).Aggregate((a, b) => string.Concat(a, ",", b)));
            }

            return string.Concat(Name, ": ", _values?.FirstOrDefault());
        }
    }
}