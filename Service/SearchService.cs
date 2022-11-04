using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using AzureSearch.Models;
using AzureSearch.SearchInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSearch.Service
{
    public class SearchService : ISearch
    {
        // Local variables
        private readonly string SearchServiceUri;
        private readonly string SearchServiceQueryApiKey;
        private static SearchIndexClient _indexClient;
        private static SearchClient _searchClient;
        public SearchService(IConfiguration configuration)
        {
            SearchServiceUri= configuration.GetSection("SearchServiceSetting").GetSection("SearchServiceUri").Value;
            SearchServiceQueryApiKey = configuration.GetSection("SearchServiceSetting").GetSection
                ("SearchServiceQueryApiKey").Value;
        }


        /// <summary>
        /// Prepare the Data for the Facet
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        public SearchData PrepareFacetData(SearchData DataModel)
        {

            List<string> facetTag = DataModel.resultList.Facets["Tags"].Select(x => x.Value.ToString()).ToList();

            var JoinStringTag = string.Join(",", facetTag).Split(",");           
            var ListAmenities = new List<AmenityFacetModel>();

            for (int i = 0; i < JoinStringTag.Length; i++)
            {             
                ListAmenities.Add(new AmenityFacetModel { FacetName = FirstLetterToUpper(JoinStringTag[i].Trim()), FacetIndex = i, FacetCount = 1 });
            }

            var DistinctGroupAmenities = ListAmenities.GroupBy(x => x.FacetName).Select(y => new { FacetName = y.Key, Count = y.Sum(z => z.FacetCount) });
            var facetAmenity = DistinctGroupAmenities.Select(x => new SelectListItem { Text = x.FacetName, Value = x.Count.ToString() }).ToList();

            var facetCategory = DataModel.resultList.Facets["Category"].Select(x => new SelectListItem { Text = x.Value.ToString(), Value = x.Count.ToString() }).ToList();

            DataModel.CategoryFacet = facetCategory;
            DataModel.AmenityFacet = facetAmenity;
            return DataModel;
        }
        /// <summary>
        /// Prepare the Initial Data
        /// </summary>
        /// <returns></returns>
        public async Task<SearchData> PrepareInitialData()
        {
            InitSearch();
            SearchOptions options = new SearchOptions();
            options.Facets.Add("Tags,count:6");
            options.Facets.Add("Category,count:5");

            SearchData model = new SearchData();
            SearchResults<Hotel> searchResult = await _searchClient.SearchAsync<Hotel>("*", options);         
            model.resultList = searchResult;
            return model;
        }
        /// <summary>
        /// Changes the 1st letter to uppercase
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        /// <summary>
        /// PreSetFacets
        /// </summary>
        /// <param name="model"></param>
        /// <param name="saveChecks"></param>
        public SearchData PreSetFacets(SearchData model, string[] CategoryFacet, string[] AmenitiesFacet)
        {

            foreach (var item in CategoryFacet)
            {
                model.CategoryFacet.Where(x => x.Text == item).FirstOrDefault().Selected = true;
            }
            foreach (var item in AmenitiesFacet)
            {
                model.AmenityFacet.Where(x => x.Text == item).FirstOrDefault().Selected = true;
            }
            return model;
        }

        /// <summary>
        /// AzureSearchFilterLikeCondition
        /// </summary>
        /// <param name="values"></param>
        /// <param name="field"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public string AzureSearchFilterLikeCondition(List<string> values, string field, bool exclude)
        {
            var result = "";
            var builder = new StringBuilder();
            if (!exclude)
            {
                if (values != null && values.Count > 0)
                {
                    var filtersValuesClubbed = string.Join(",", values);
                    builder.Append($"search.ismatch('{filtersValuesClubbed}','{ field }')");
                }
            }
            else
            {
                if (values != null && values.Count > 0)
                {
                    var filtersValuesClubbed = string.Join(",", values);
                    builder.Append($"not search.ismatch('{filtersValuesClubbed}','{ field }')");
                }
            }
            result = builder.ToString();
            if (string.IsNullOrWhiteSpace(result) && values != null)
            {
                result = "";
            }
            return result;
        }

        public async Task<SearchData> SearchHelper(SearchData model,string TagsFilter,string CategoryFilter,int page)
        {
            var options = new SearchOptions
            {

                SearchMode = SearchMode.All,
                Skip = page * GlobalVariables.ResultsPerPage,
                Size = GlobalVariables.ResultsPerPage,
                IncludeTotalCount = true,
            };
            options.Select.Add("HotelName");
            options.Select.Add("Description");
            options.Select.Add("Tags");
            options.Select.Add("Category");
            options.Facets.Add("Tags,count:6");
            options.Facets.Add("Category,count:5");
            var filterbuilder = new StringBuilder();
            filterbuilder.Append(string.IsNullOrEmpty(TagsFilter) ? "" : filterbuilder.Length > 0 ? $"{TagsFilter}" : $"{TagsFilter}");
            filterbuilder.Append(string.IsNullOrEmpty(CategoryFilter) ? "" : filterbuilder.Length > 0 ? $" and {CategoryFilter}" : $"{CategoryFilter}");
            options.Filter = filterbuilder.ToString();
            model.resultList = await _searchClient.SearchAsync<Hotel>(model.searchText, options);
            return model;
        }

        /// <summary>
        /// To setup the search client and all the configurations
        /// </summary>
        public SearchClient InitSearch()
        {
         //   _builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
         //   _configuration = _builder.Build();
         //   string searchServiceUri = _configuration["SearchServiceUri"];
         //   string queryApiKey = _configuration["SearchServiceQueryApiKey"];
            _indexClient = new SearchIndexClient(new Uri(SearchServiceUri), new AzureKeyCredential(SearchServiceQueryApiKey));
            _searchClient = _indexClient.GetSearchClient("hotels-search-index");
            return _searchClient;
        }
    }
}
