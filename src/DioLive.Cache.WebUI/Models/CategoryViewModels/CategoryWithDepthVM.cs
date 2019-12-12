using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class CategoryWithDepthVM
	{
		public CategoryWithDepthVM(Hierarchy<Category, int>.Node categoryNode,
		                           IEnumerable<Category> parentCandidates)
		{
			Id = categoryNode.Value.Id;
			ParentId = categoryNode.Value.ParentId;
			Name = categoryNode.Value.Name;
			Color = categoryNode.Value.Color.ToString("X6");
			Depth = categoryNode.Level;
			ParentCandidates = parentCandidates.Select(ca => new CategoryVM
			{
				Id = ca.Id,
				Name = ca.Name,
				Color = ca.Color.ToString("X6")
			}).ToArray();
		}

		public int Id { get; }

		public int? ParentId { get; }

		public string Name { get; }

		public string Color { get; }

		public int Depth { get; }

		public IReadOnlyCollection<CategoryVM> ParentCandidates { get; }
	}
}