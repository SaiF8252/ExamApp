using ExamApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamApp.ViewModels
{
    public class InvoiceVm
    {
        public int InvId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true, NullDisplayText = "select Invoice Date")]
        [DisplayName("Invoice Date")]
        public System.DateTime InvDate { get; set; }=DateTime.Now;
        public string CustomerName { get; set; }
        public string Salesman { get; set; }

       public List<InvoiceDetail> InvoiceDetails { get; set; }=new List<InvoiceDetail> ();
      
        public InvoiceVm()
        {
            
        }
        public InvoiceVm(Patientmaster master)
        {
            this.InvId = master.InvoiceId;
            this.CustomerName = master.CustomerName;
            this.Salesman = master.Salesman;
             this.InvDate = master.InvoiceDate;
            this.InvoiceDetails=master.InvoiceDetails.ToList();
        }
        public Patientmaster Convert()
        {

            Patientmaster model = new Patientmaster();
            model.InvoiceId = this.InvId;
            model.CustomerName = this.CustomerName;
            model.Salesman = this.Salesman;
            model.InvoiceDate = this.InvDate;
           
            model.InvoiceDetails = this.InvoiceDetails;
            return model;
        }
    }
}
