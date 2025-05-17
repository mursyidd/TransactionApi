using Microsoft.AspNetCore.Mvc;
using TransactionApi.Models;
using TransactionApi.Utils;

namespace TransactionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmitTrxMessageController : ControllerBase
{
    private static readonly Dictionary<string, string> allowedPartners = new()
    {
        { "FAKEGOOGLE", "RkFLRVBBU1NXT1JEMTIzNA==" },
        { "FAKEPEOPLE", "RkFLRVBBU1NXT1JENTc4" }
    };

    [HttpPost]
    public IActionResult Submit([FromBody] TransactionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.partnerkey))
            return BadRequest(new { result = 0, resultmessage = "partnerkey is Required." });

        if (string.IsNullOrWhiteSpace(request.partnerrefno))
            return BadRequest(new { result = 0, resultmessage = "partnerrefno is Required." });

        if (string.IsNullOrWhiteSpace(request.partnerpassword))
            return BadRequest(new { result = 0, resultmessage = "partnerpassword is Required." });

        if (string.IsNullOrWhiteSpace(request.timestamp))
            return BadRequest(new { result = 0, resultmessage = "timestamp is Required." });

        if (string.IsNullOrWhiteSpace(request.sig))
            return BadRequest(new { result = 0, resultmessage = "sig is Required." });

        if (!allowedPartners.TryGetValue(request.partnerkey, out var expectedPassword) ||
            request.partnerpassword != expectedPassword)
        {
            return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
        }

        if (!SignatureHelper.VerifySignature(request, expectedPassword))
        {
            return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
        }

        if (!TimeHelper.IsTimestampValid(request.timestamp))
        {
            return BadRequest(new { result = 0, resultmessage = "Expired." });
        }

        if (request.items != null && request.items.Count > 0)
        {
            var itemsTotal = request.items.Sum(i => i.qty * i.unitprice);
            if (itemsTotal != request.totalamount)
            {
                return BadRequest(new { result = 0, resultmessage = "Invalid Total Amount." });
            }

            foreach (var item in request.items)
            {
                if (string.IsNullOrWhiteSpace(item.partneritemref))
                    return BadRequest(new { result = 0, resultmessage = "partneritemref is Required." });

                if (string.IsNullOrWhiteSpace(item.name))
                    return BadRequest(new { result = 0, resultmessage = $"name for {item.partneritemref} is Required." });

                if (item.qty < 1)
                    return BadRequest(new { result = 0, resultmessage = "quantity must be more than 0" });

                if (item.unitprice <= 0)
                    return BadRequest(new { result = 0, resultmessage = "unitprice must be positive." });
            }
        }

        long totalDiscount = DiscountHelper.CalculateDiscount(request.totalamount);
        long finalAmount = request.totalamount - totalDiscount;

        return Ok(new
        {
            result = 1,
            request.totalamount,
            totaldiscount = totalDiscount,
            finalamount = finalAmount
        });
    }
}