using System.Collections.Generic;

namespace DioLive.Cache.WebUI.Models.BudgetSharingViewModels
{
    public class BudgetSharingsVM
    {
        public ICollection<Share> Shares { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}