using System.ComponentModel.DataAnnotations;

using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.WebApp.Models.BudgetSharingViewModels;

public class ShareVM
{
    public Guid BudgetId { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public required string UserName { get; set; }

    public ShareAccess Access { get; set; } = ShareAccess.ReadOnly;
}