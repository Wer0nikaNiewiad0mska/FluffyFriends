using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Exceptions;

public class CardNumberInvalidException : Exception
{
    public CardNumberInvalidException() { }
    public CardNumberInvalidException(string message) : base("Card number is invalid.") { }
    public CardNumberInvalidException(Exception innerException) : base("Card number is invalid.", innerException) { }
}
