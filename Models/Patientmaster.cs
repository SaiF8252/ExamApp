namespace ExamApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Patientmaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        public string CustomerName { get; set; }

        public string Salesman { get; set; }

        [NotMapped]
        public decimal Total
        {
            get
            {
                // InvoiceDetails থেকে auto calculate
                return InvoiceDetails?.Sum(x => x.TotalAmount) ?? 0;
            }
        }

        // Navigation property
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
