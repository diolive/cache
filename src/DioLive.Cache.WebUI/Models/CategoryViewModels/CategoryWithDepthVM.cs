using System.Collections.Generic;
using System.Linq;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.WebUI.Models.CategoryViewModels
{
	public class CategoryWithDepthVM
	{
		public CategoryWithDepthVM(Hierarchy<Category, int>.Node categoryNode,
								   IReadOnlyCollection<CategoryLocalization> localizations,
								   IEnumerable<Category> parentCandidates)
		{
			Id = categoryNode.Value.Id;
			ParentId = categoryNode.Value.ParentId;
			Name = categoryNode.Value.Name;
			Localizations = localizations;
			Color = categoryNode.Value.Color.ToString("X6");
			Depth = categoryNode.Level;
			ParentCandidates = parentCandidates.Select(ca => new CategoryVM
			{
				Id = ca.Id,
				DisplayName = ca.Name,
				Color = ca.Color.ToString("X6")
			}).ToArray();
		}

		public int Id { get; }

		public int? ParentId { get; }

		public string Name { get; }

		public IReadOnlyCollection<CategoryLocalization> Localizations { get; }

		public string Color { get; }

		public int Depth { get; }

		public IReadOnlyCollection<CategoryVM> ParentCandidates { get; }
	}
}