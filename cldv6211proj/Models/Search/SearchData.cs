using Azure.Search.Documents.Models;

namespace cldv6211proj.Models.Search
{
    // source/reference: https://learn.microsoft.com/en-us/azure/search/tutorial-csharp-create-mvc-app
    public class SearchData
    {
        // The text to search for.
        public string searchText { get; set; }

        // The list of results.
        public SearchResults<ProductSearch> resultList;
    }
}
