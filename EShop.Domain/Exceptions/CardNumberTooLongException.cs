using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Exceptions;

public class CardNumberTooLongException : Exception
{
    public CardNumberTooLongException() { }
    public CardNumberTooLongException(string message) : base("Card number is too long.") { }
    public CardNumberTooLongException(Exception innerException) : base("Card number is too long.", innerException) { }
}
