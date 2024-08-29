using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STOCKTRACKING.Models.Entity;
namespace STOCKTRACKING.Models.UrunModels
{
    public class UrunFormModel
    {
        public STOCK STOCK { get; set; } = new STOCK();
        public List<CATEGORY> CATEGORY { get; set; } = new List<CATEGORY>();

        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<STOCKCATEGORYS>  STOCKCATEGORYS { get; set; }= new List<STOCKCATEGORYS>();
    }

}