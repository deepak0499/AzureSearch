using Azure.Search.Documents;
using AzureSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.SearchInterface
{
    public interface ISearch
    {
        public SearchData PrepareFacetData(SearchData DataModel);
        public Task<SearchData> PrepareInitialData();
        public string FirstLetterToUpper(string str);
        public SearchData PreSetFacets(SearchData model, string[] CategoryFacet, string[] AmenitiesFacet);
        public string AzureSearchFilterLikeCondition(List<string> values, string field, bool exclude);

        public Task<SearchData> SearchHelper(SearchData model, string TagsFilter, string CategoryFilter,int page);
        public SearchClient InitSearch();
    }
}
