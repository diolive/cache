using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }

        [Required, StringLength(300)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = Constants.DateDisplayFormat, ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = Constants.CostDisplayFormat)]
        public int Cost { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        public string Shop { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public string LastEditorId { get; set; }

        public string Comments { get; set; }

        [DisplayFormat(DataFormatString = Constants.DateUTCDisplayFormat, ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }

        public Guid BudgetId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ApplicationUser LastEditor { get; set; }

        public virtual Budget Budget { get; set; }

        public static Task<Purchase> GetWithShares(Data.ApplicationDbContext context, Guid id)
        {
            return context.Purchase
                .Include(p => p.Author)
                .Include(p => p.LastEditor)
                .Include(c => c.Budget)
                .ThenInclude(b => b.Shares)
                .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}