using System;
using System.ComponentModel.DataAnnotations;

namespace DioLive.Cache.WebUI.Models.BudgetSharingViewModels
{
	public class NewShareVM
	{
		public Guid BudgetId { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string UserName { get; set; }

		public ShareAccess Access { get; set; }
	}
}