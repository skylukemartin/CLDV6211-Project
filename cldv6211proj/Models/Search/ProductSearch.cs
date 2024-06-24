using System.ComponentModel.DataAnnotations;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace cldv6211proj.Models.Search
{
    // source/reference: https://learn.microsoft.com/en-us/azure/search/tutorial-csharp-create-mvc-app
    public partial class ProductSearch
    {
        [SimpleField(IsKey = true, IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string ProductID { get; set; }

        [SearchableField(IsSortable = true, IsFilterable = true, IsFacetable = true)]
        public string ProductName { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public double ProductPrice { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public int ProductAvailability { get; set; }

        [SearchableField(
            IsSortable = true,
            IsFilterable = true,
            IsFacetable = true,
            AnalyzerName = LexicalAnalyzerName.Values.StandardLucene
        )]
        public string ProductDescription { get; set; }

        [SearchableField(
            IsSortable = true,
            IsFilterable = true,
            IsFacetable = true,
            AnalyzerName = LexicalAnalyzerName.Values.StandardLucene
        )]
        public string ProductCategory { get; set; }

        [SearchableField(
            IsSortable = true,
            IsFilterable = true,
            IsFacetable = true,
            AnalyzerName = LexicalAnalyzerName.Values.StandardLucene
        )]
        public string ProductImageURL { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public int UserID { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string[] Keyphrases { get; set; }
    }
}
