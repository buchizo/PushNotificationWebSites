namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;

    public enum ListConstraintType
    {
        Exclude,
        Include
    }

    public class ListConstraint : IRouteConstraint
    {
        public ListConstraint()
            : this(ListConstraintType.Include, new string[] { })
        {
        }

        public ListConstraint(params string[] list)
            : this(ListConstraintType.Include, list)
        {
        }

        public ListConstraint(ListConstraintType listType, params string[] list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            this.ListType = listType;
            this.List = new List<string>(list);
        }

        public ListConstraintType ListType { get; private set; }

        public IList<string> List { get; private set; }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var value = values[parameterName.ToLowerInvariant()] as string;
            var found = this.List.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase));

            return this.ListType == ListConstraintType.Include ? found : !found;
        }
    }
}