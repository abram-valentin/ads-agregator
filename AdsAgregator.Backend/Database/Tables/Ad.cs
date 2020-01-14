using AdsAgregator.CommonModels.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsAgregator.Backend.Database.Tables
{
    public class Ad : AdModel
    {
        [Key]
        public override int Id { get; set; }
        
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
