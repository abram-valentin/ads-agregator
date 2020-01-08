using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsAgregator.Backend.Database.Tables
{
    public class SearchItem : CommonModels.Models.SearchItem
    {
        [Key]
        public int Id { get; set; }
   

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
