﻿using System;
using System.ComponentModel.DataAnnotations;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.WebUI.Models.BudgetSharingViewModels
{
	public class ShareVM
	{
		public Guid BudgetId { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string UserName { get; set; }

		public ShareAccess Access { get; set; }
	}
}