using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Reso.Sdk.Core.Attributes;

namespace Reso.Sdk.Core.ViewModels
{
    public abstract class SortModel
    {
        [JsonIgnore] [SortDirection] 
        [Skip]
        public string SortDirection { get; set; }
        [JsonIgnore] [SortProperty] 
        [Skip]
        public string  SortProperty { get; set; }
    }
}