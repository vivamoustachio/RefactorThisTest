using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoiceService(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.Get(payment.Reference);
			var responseMessage = string.Empty;

			if (inv == null)
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

            CheckState(inv);
            if (inv.Payments != null && inv.Payments.Any())
            {
                if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                {
                    responseMessage = "invoice was already fully paid";
                }
                else if (inv.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
                {
                    responseMessage = "the payment is greater than the partial amount remaining";
                }
                else
                {
                    if ((inv.Amount - inv.AmountPaid) == payment.Amount)
                    {
                        inv.AmountPaid += payment.Amount;
                        if (inv.Type == InvoiceType.Commercial)
                        {
                            inv.TaxAmount += payment.Amount * 0.14m;
                        }
                        inv.Payments.Add(payment);
                        responseMessage = "final partial payment received, invoice is now fully paid";
                    }
                    else
                    {
                        inv.AmountPaid += payment.Amount;
                        if (inv.Type == InvoiceType.Commercial)
                        {
                            inv.TaxAmount += payment.Amount * 0.14m;
                        }
                        inv.Payments.Add(payment);
                        responseMessage = "another partial payment received, still not fully paid";
                    }
                }
            }
            else
            {
                inv.AmountPaid = payment.Amount;
                inv.TaxAmount = payment.Amount * 0.14m;
                inv.Payments.Add(payment);

                if (payment.Amount > inv.Amount)
                {
                    responseMessage = "the payment is greater than the invoice amount";
                }
                else if (inv.Amount == payment.Amount)
                {
                    responseMessage = "invoice is now fully paid";
                }
                else
                {
                    responseMessage = "invoice is now partially paid";
                }                
            }
            inv.Save();
			return responseMessage;
		}

        public void CheckState(Invoice invoice)
        {
            if (invoice.Amount == 0)
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                {
                    throw new InvalidOperationException("no payment needed"); //no point continuing the logic here as the payments are not required.
                }
                else
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
        }
    }
}