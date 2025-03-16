using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence {

	//missing persistance to EF, which I'd recommend.

	public class InvoiceRepository
	{
		private List<Invoice> _invoices = new List<Invoice>(); //database collection... for EF etc.

		public Invoice Get(string reference) //Only require verbs as the subject is explicit in the class name.
		{
			return _invoices.FirstOrDefault(p => p.Reference == reference);
		}

		public void Save(Invoice invoice)
		{
            //saves the invoice to the database
            Invoice existingInvoice = _invoices.FirstOrDefault(p => p.Id == invoice.Id);
            if (existingInvoice == null) //reference should be updatable so have added Id.
            {
                Add(invoice);
            }
            else
            {
                _invoices[_invoices.IndexOf(existingInvoice)] = invoice;
            }
		}

		public void Add(Invoice invoice)
		{
			_invoices.Add(invoice); //updated to Linq/list syntax.
		}
	}
}