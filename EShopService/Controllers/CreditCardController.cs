using Microsoft.AspNetCore.Mvc;
using EShop.Domain.Exceptions;
using EShop.Application;
using EShop.Application.Interfaces;

namespace EShopService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditCardController : ControllerBase
{
    private readonly ICreditCardService _creditCardService;

    public CreditCardController(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    [HttpGet("validate")]
    public IActionResult Validate(string cardNumber)
    {
        try
        {
            bool isValidLength = _creditCardService.ValidateLength(cardNumber);
            if (!isValidLength)
            {
                return BadRequest("Card number is invalid.");
            }

            _creditCardService.ValidateCard(cardNumber);

            string issuer = _creditCardService.GetCardType(cardNumber);
            return Ok(new { status = "valid", issuer });
        }
        catch (CardNumberTooLongException ex)
        {
            return StatusCode(414, ex.Message);
        }
        catch (CardNumberTooShortException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (CardNumberInvalidException ex)
        {
            return StatusCode(406, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
        }
    }
}
