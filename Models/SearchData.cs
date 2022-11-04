using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AzureSearch.Models
{
    public static class GlobalVariables
    {
        public static int ResultsPerPage
        {
            get
            {
                return 7;
            }
        }
    }
    public class SearchData
    {
        public SearchData()
        {
        }

        // Constructor to initialize the list of facets sent from the controller.
     /*   public SearchData(List<string> mylist, List<string> facetCategory,List<int> mycount)
        {
            facetAmenity = new string[mylist.Count];
            facetcategory = new string[facetCategory.Count];
            Mycount = new int[mycount.Count];

            for (int i = 0; i < mylist.Count; i++)
            {
                facetAmenity[i] = mylist[i];
            }
            for (int j = 0; j < facetCategory.Count; j++)
            {
                facetcategory[j] = facetCategory[j];
            }
            for (int k = 0; k < mycount.Count; k++)
            {
                Mycount[k] = mycount[k];
            }
        }
     */
        // Array to hold the text for each amenity.
      //  public string[] facetAmenity { get; set; }

      //  public int[] Mycount { get; set; }

     //   public int[] MyCatcount { get; set; }

     //   public string[] facetcategory { get; set; }

        // Array to hold the setting for each amenitity.
     //   public bool[] facetOn { get; set; }

     //   public bool[] facetCatOn { get; set; }

        // The text to search for.
        public string searchText { get; set; }

        // Record if the next page is requested.
        public string paging { get; set; }

        // The list of results.
        public SearchResults<Hotel> resultList;

        public List<SelectListItem> CategoryFacet { get; set; }
        public List<SelectListItem> AmenityFacet { get; set; }



        //   public string scoring { get; set; }
    }
 
}
public class AmenityFacetModel
{
    public string FacetName { get; set; }
    public int FacetIndex { get; set; }

    public int FacetCount { get; set; }
}
