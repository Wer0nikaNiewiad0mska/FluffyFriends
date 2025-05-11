using System.ComponentModel.DataAnnotations;
using Xunit;
using EShop.Domain.Exceptions;

namespace EShop.Application.Tests;

public class CreditCardServiceTest
{

    [Fact]
    public void ValidateCard_NumberTooShort_ThrowsWithExpectedMessage()
    {
        var service = new CreditCardService();
        string cardNumber = "123456789012";

        var exception = Assert.Throws<CardNumberTooShortException>(() => service.ValidateLength(cardNumber));

        Assert.Equal("Card number is too short.", exception.Message);
    }

    [Fact]
    public void ValidateCard_NumberTooLong_ThrowsWithExpectedMessage()
    {
        var service = new CreditCardService();
        string cardNumber = "12345678901234567890";

        var exception = Assert.Throws<CardNumberTooLongException>(() => service.ValidateLength(cardNumber));

        Assert.Equal("Card number is too long.", exception.Message);
    }
    [Fact]
    public void CreditCardService_Perfect_ReturnsTrue()
    {
        var service = new CreditCardService();
        string cardNumber = "123456789012345";

        bool isValid = service.ValidateLength(cardNumber);

        Assert.True(isValid);
    }

    [Theory]
    [InlineData("4024007165401778")]
    [InlineData("4024 0071 6540 1778")]
    [InlineData("4024-0071-6540-1778")]
    public void ValidateCard_AllowsDifferentFormatting_ReturnsTrue(string cardNumber)
    {
        var service = new CreditCardService();

        bool result = service.ValidateLength(cardNumber);

        Assert.True(result);
    }

    [Theory]
    [InlineData("4024%0071%6540%1778")]
    public void ValidateCard_AllowsDifferentFormatting_ThrowsWithExpectedMessage(string cardNumber)
    {
        var service = new CreditCardService();

        var exception = Assert.Throws<CardNumberInvalidException>(() => service.ValidateLength(cardNumber));

        Assert.Equal("Card number is invalid.", exception.Message);
    }

    [Theory]
    [InlineData("4111 1111 1111 1111")]
    public void ValidateCard_ValidNumber_ReturnsTrue(string cardNumber)
    {
        var service = new CreditCardService();

        bool result = service.ValidateCard(cardNumber);

        Assert.True(result);
    }

    [Theory]
    [InlineData("4111 1111 1111 1112")]
    [InlineData("4111-1111-1111-1110")]
    [InlineData("4111111111111112")]
    public void ValidateCard_InvalidNumber_ThrowsCardNumberInvalidException(string cardNumber)
    {
        var service = new CreditCardService();

        var exception = Assert.Throws<CardNumberInvalidException>(() => service.ValidateCard(cardNumber));

        Assert.Equal("Card number is invalid.", exception.Message);
    }
    [Fact]
    public void GetCardType_Visa_ReturnsVisa()
    {
        var service = new CreditCardService();
        string cardNumber = "4024-0071-6540-1778";

        string type = service.GetCardType(cardNumber);

        Assert.Equal("Visa", type);
    }

    [Fact]
    public void GetCardType_MasterCard_ReturnsMasterCard()
    {
        var service = new CreditCardService();
        string cardNumber = "5530016454538418";

        string type = service.GetCardType(cardNumber);

        Assert.Equal("MasterCard", type);
    }

    [Fact]
    public void GetCardType_AmericanExpress_ReturnsAmericanExpress()
    {
        var service = new CreditCardService();
        string cardNumber = "378523393817437";

        string type = service.GetCardType(cardNumber);

        Assert.Equal("American Express", type);
    }

    [Fact]
    public void GetCardType_InvalidNumber_ThrowsWithExpectedMessage()
    {
        var service = new CreditCardService();
        string cardNumber = "3528869865563675";

        var exception = Assert.Throws<CardNumberInvalidException>(() => service.GetCardType(cardNumber));

        Assert.Equal("Card number is invalid.", exception.Message);
    }
}