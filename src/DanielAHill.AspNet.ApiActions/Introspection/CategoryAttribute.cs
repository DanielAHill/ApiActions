using System;
using System.Collections.Generic;
using DanielAHill.AspNet.ApiActions.Introspection;

// ReSharper disable once CheckNamespace
namespace DanielAHill.AspNet.ApiActions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute: Attribute, IHasCategories
    {
        public IReadOnlyCollection<string> Tags { get; }

        public CategoryAttribute(params string[] tags)
        {
            Tags = tags;
        }
    }
}
