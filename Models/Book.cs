using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace asn1.Models {
    public class Book {
        [Key]
        public string BookID { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        [Display(Name = "Published Date")]
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        [Display (Name = "ISBN")]
        public int ISBN_10 { get; set; }
    }
}