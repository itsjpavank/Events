using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEvents
{
    [ExcludeFromCodeCoverage]
    public class Event
    {
        public string EventId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public string LocationName { get; set; }
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public string[] EventsFor { get; set; }
        public string[] PrimaryLanguage { get; set; }
        public string[] Product { get; set; }
        public bool? IsPublishable { get; set; }
        public string Icon { get; set; }
        public string[] DescriptionHighlights { get; set; }
        public string[] TitleHighlights { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }        
        public string RegistrationFees { get; set; }
        public string LanguageCode { get; set; }
    }
}
