using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BrickSet.Core
{

    /// <summary>
    /// Lego set theme.
    /// </summary>
    [DataContract(Name = "themes", Namespace = "")]
    public class Theme
    {

        /// <summary>
        /// The name of the theme.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [DataMember(Name = "theme", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Number of sets contained in the theme. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        [DataMember(Name = "setCount", EmitDefaultValue = false)]
        public int SetCount { get; set; }

        /// <summary>
        /// Number of subthemes contained in the theme. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        [DataMember(Name = "subthemeCount", EmitDefaultValue = false)]
        public int SubthemeCount { get; set; }

        /// <summary>
        /// Year that the theme first had a set. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        [DataMember(Name = "yearFrom", EmitDefaultValue = false)]
        public int YearFrom { get; set; }

        /// <summary>
        /// Year that the theme last had a set. 
        /// </summary>
        [DataMember(Name = "yearTo", EmitDefaultValue = false)]
        public int YearTo { get; set; }


    }

    [DataContract(Name = "additionalImages", Namespace = "")]
    public class AdditionalImage
    {
        /// <summary>
        /// The name of the theme.
        /// </summary>
        [DataMember(Name = "imageURL", EmitDefaultValue = false)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The name of the theme.
        /// </summary>
        [DataMember(Name = "largeThumbnailURL", EmitDefaultValue = false)]
        public string LargeThumbnailUrl { get; set; }

        /// <summary>
        /// The name of the theme.
        /// </summary>
        [DataMember(Name = "thumbnailURL", EmitDefaultValue = false)]
        public string ThumbnailUrl { get; set; }

    }

    [CollectionDataContract(Name="ArrayOfThemes", Namespace = "")]
    public class ThemesList: List<Theme>
    {
    }

    [CollectionDataContract(Name = "ArrayOfAdditionalImages", Namespace = "")]
    public class AdditionalImagesList : List<AdditionalImage>
    {
    }

}

