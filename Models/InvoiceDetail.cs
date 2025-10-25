namespace ExamApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class InvoiceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Sl { get; set; }

        [ForeignKey("Patientmaster")]
        public int InvoiceId { get; set; }

        [ForeignKey("Product")]
        public byte ItemId { get; set; }

        public int? StripQuantity { get; set; }
        public int? UnitQuantity { get; set; }

        [NotMapped]
        public decimal TotalAmount
        {
            get
            {
                decimal StripTotal = StripQuantity.HasValue ? StripQuantity.Value * Product.StripPrice : 0;
                decimal UnitTotal = UnitQuantity.HasValue ? UnitQuantity.Value * Product.UnitPrice : 0;
                return StripTotal + UnitTotal;
            }
        }

        public virtual Patientmaster Patientmaster { get; set; }
        public virtual Product Product { get; set; }
    }
}
