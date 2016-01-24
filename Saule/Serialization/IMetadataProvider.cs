using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Saule.Queries.Pagination;

namespace Saule.Serialization
{
    internal abstract class EnumerablePaginationProvider : IPaginationProvider
    {
        public PaginationResult Paginate(
            object value,
            Type valueType,
            ApiResource resourceType,
            IEnumerable<PaginationLinkParameter> parameters)
        {
            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            var test = methods.Where(m => m.ContainsGenericParameters && m.Name == "Paginate");

            var enumerable = test.Single(m => m.GetParameters().First().ParameterType == typeof(IEnumerable<>));

            return enumerable.Invoke(this, new[] { value, resourceType, parameters }) as PaginationResult;
        }

        protected abstract PaginationResult Paginate<T>(
            IEnumerable<T> collection,
            ApiResource resourceType,
            IEnumerable<PaginationLinkParameter> parameters);

        protected abstract PaginationResult Paginate<T>(
            IQueryable<T> collection,
            ApiResource resourceType,
            IEnumerable<PaginationLinkParameter> parameters);
    }

    internal class SimplePaginationProvider : EnumerablePaginationProvider
    {
        public SimplePaginationProvider(int defaultPerPage, bool allowOverride)
        {
        }

        protected override PaginationResult Paginate<T>(IEnumerable<T> collection, ApiResource resourceType, IEnumerable<PaginationLinkParameter> parameters)
        {
            return new PaginationResult
            {
                Result = collection.Skip(10).Take(10)
            };
        }

        protected override PaginationResult Paginate<T>(IQueryable<T> collection, ApiResource resourceType, IEnumerable<PaginationLinkParameter> parameters)
        {
        }
    }

    internal interface IPaginationProvider
    {
        PaginationResult Paginate(
            object value,
            Type valueType,
            ApiResource resourceType,
            IEnumerable<PaginationLinkParameter> parameters);
    }

    internal sealed class PaginationResult
    {
        public object Result { get; set; }

        public ICollection<PaginationLinkParameter> PreviousLink { get; } = new List<PaginationLinkParameter>();

        public ICollection<PaginationLinkParameter> NextLink { get; } = new List<PaginationLinkParameter>();

        public ICollection<PaginationLinkParameter> FirstLink { get; } = new List<PaginationLinkParameter>();

        public ICollection<PaginationLinkParameter> LastLink { get; } = new List<PaginationLinkParameter>();
    }

    internal sealed class PaginationLinkParameter
    {
        public PaginationLinkParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        internal IEnumerable<PaginationLinkParameter> FromUrlQuery(
            IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            return queryParameters
                .Where(q => q.Key.StartsWith("page."))
                .Select(q => new PaginationLinkParameter(q.Key.Remove(0, 5), q.Value));
        }
    }
}
