using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Application.Interfaces;

public interface ICreditCardService
{
    bool ValidateLength(string cardNumber);
    bool ValidateCard(string cardNumber);
    string GetCardType(string cardNumber);
}
