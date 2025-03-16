using System.Collections.Generic;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        private readonly InvoiceRepository _repository;

        public Invoice(InvoiceRepository repository)
        {
            _repository = repository;
        }

        public void Save()
        {
            _repository.Save(this);
        }

        public long Id { get; set; } //ID required for explicit matching
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public string Reference { get; set; } //Added for continuity of the example workflow.
        public List<Payment> Payments { get; set; } = new List<Payment>(); //default initialization to avoid null reference from somewhere obscure.
        public InvoiceType Type { get; set; }
    }

    public enum InvoiceType
    {
        Standard,
        Commercial
    }
}