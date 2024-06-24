using System.Diagnostics;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using cldv6211proj.Models;
using cldv6211proj.Models.Database;
using cldv6211proj.Models.Search;
using cldv6211proj.Models.ViewModels;
using cldv6211proj.Services;
using Microsoft.AspNetCore.Mvc;

namespace cldv6211proj.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult MyWork()
        {
            return View(new ProductList() { Products = _productService.GetProducts() });
        }

        [HttpGet]
        public IActionResult CraftProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult FinishCraftProduct(Product product)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ContactUs", "Home");
            var userID = HttpContext.Session.GetInt32("userID") ?? -1;
            if (userID < 1)
                return RedirectToAction("Login", "Home");
            product.UserID = userID;
            if (!_productService.AddProduct(product))
                return RedirectToAction("ContactUs", "Home");
            return RedirectToAction("OrderHistory", "Order");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }

        // source/reference: https://learn.microsoft.com/en-us/azure/search/tutorial-csharp-create-mvc-app

        public IActionResult SearchProducts()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SearchProducts(SearchData model)
        {
            try
            {
                // Check for a search string
                if (model.searchText == null)
                {
                    model.searchText = "";
                }

                // Send the query to Search.
                await RunQueryAsync(model);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }
            return View(model);
        }

        private static SearchClient _searchClient;
        private static SearchIndexClient _indexClient;
        private static IConfigurationBuilder _builder;
        private static IConfigurationRoot _configuration;

        private void InitSearch()
        {
            // Create a configuration using appsettings.json
            _builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _configuration = _builder.Build();

            // Read the values from appsettings.json
            string searchServiceUri = _configuration["SearchServiceUri"];
            string queryApiKey = _configuration["SearchServiceQueryApiKey"];

            // Create a service and index client.
            _indexClient = new SearchIndexClient(
                new Uri(searchServiceUri),
                new AzureKeyCredential(queryApiKey)
            );
            _searchClient = _indexClient.GetSearchClient("azuresql-index");
        }

        private async Task<ActionResult> RunQueryAsync(SearchData model)
        {
            InitSearch();

            var options = new SearchOptions() { IncludeTotalCount = true };

            // Enter Product property names to specify which fields are returned.
            // If Select is empty, all "retrievable" fields are returned.
            // options.Select.Add("ProductName");
            // options.Select.Add("ProductPrice");
            // options.Select.Add("ProductAvailability");
            // options.Select.Add("ProductDescription");
            // options.Select.Add("ProductCategory");
            // options.Select.Add("ProductImageURL");
            // options.Select.Add("UserID");

            // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
            model.resultList = await _searchClient
                .SearchAsync<ProductSearch>(model.searchText, options)
                .ConfigureAwait(false);

            // Display the results.
            return View("Index", model);
        }
    }
}
