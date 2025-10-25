
namespace ExamApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        public string MedicineName { get; set; }
        public string ImagePath { get; set; }
        public decimal StripPrice { get; set; }
        public int? StripSize { get; set; } = 10;
        public decimal UnitPrice { get; set; }
        public int? StockQuantity { get; set; }


        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
