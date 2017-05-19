using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GetEvents
{
    /// <summary>
    /// SearchResponse
    /// </summary>
[ExcludeFromCodeCoverage]
    public class SearchResponse
    {
        public List<Event> Events
        {
            get;
            set;
        }
        //public Facet Facets;

        public long TotalResults;
    }
}