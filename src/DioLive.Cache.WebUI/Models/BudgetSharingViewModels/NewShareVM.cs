using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Models;

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