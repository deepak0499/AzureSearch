using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using AzureSearch.Models;
using AzureSearch.SearchInterface;
using AzureSearch.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AzureSearch.Models.SearchData;

namespace AzureSearch.Controllers
{
    /// <summary>
    /// Loads the first page
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ISearch ctx;
        public HomeController(ISearch _ctx)
        {
            ctx = _ctx;
        }

        /// <summary>
        /// Reload the 1st page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = ctx.PrepareInitialData().Result;
            model = ctx.PrepareFacetData(model);
            return View(model);
        }

        /// <summary>
        /// Reload the 2nd page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(SearchData model)
        {
            try
            {
                int page;
                var CategoryFilter = string.Empty;
                var TagsFilter = string.Empty;
                ctx.InitSearch();
                if (model.paging != null && model.paging == "next")
                {
                    page = (int)TempData["page"] + 1;
                    model.searchText = TempData["searchfor"].ToString();
                }
                else
                {
                    if (model.searchText == null)
                    {
                        model.searchText = "*";
                    }
                    page = 0;
                }
                if (Request.Form["AmenityFacet"].Count > 0)
                {
                    TagsFilter = ctx.AzureSearchFilterLikeCondition(new List<string>(Request.Form["AmenityFacet"]), "Tags", false);
                }

                if (Request.Form["CategoryFacet"].Count > 0)
                {
                    CategoryFilter = ctx.AzureSearchFilterLikeCondition(new List<string>(Request.Form["CategoryFacet"]), "Category", false);
                }
                await ctx.SearchHelper(model, TagsFilter, CategoryFilter,page);
                var InitalModel = ctx.PrepareInitialData().Result;
                InitalModel = ctx.PrepareFacetData(InitalModel);
                model.AmenityFacet = InitalModel.AmenityFacet;
                model.CategoryFacet = InitalModel.CategoryFacet;
                TempData["page"] = page;
                TempData["searchfor"] = model.searchText;
                model = ctx.PreSetFacets(model, Request.Form["CategoryFacet"], Request.Form["AmenityFacet"]);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }

            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}