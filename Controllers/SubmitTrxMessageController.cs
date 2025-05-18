using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TransactionApi.Models;
using TransactionApi.Utils;

namespace TransactionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmitTrxMessageController : ControllerBase
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(SubmitTrxMessageController));

    private static readonly Dictionary<string, string> allowedPartners = new()
    {
        { "FAKEGOOGLE", "RkFLRVBBU1NXT1JEMTIzNA==" },
        { "FAKEPEOPLE", "RkFLRVBBU1NXT1JENTc4" }
    };

    [HttpPost]
    [Route("v1")]
    public IActionResult Submit1([FromBody] TransactionRequest request)
    {
        _logger.Info("Incoming request: " + JsonSerializer.Serialize(request));

        try
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
                _logger.Warn("Access denied: Invalid partner credentials");
                return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
            }

            if (!SignatureHelper.VerifySignature(request, expectedPassword))
            {
                _logger.Warn("Access denied: Signature mismatch");
                return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
            }

            if (!TimeHelper.IsTimestampValid(request.timestamp))
            {
                _logger.Warn("Expired request timestamp: " + request.timestamp);
                return BadRequest(new { result = 0, resultmessage = "Expired." });
            }

            if (request.items != null && request.items.Count > 0)
            {
                var itemsTotal = request.items.Sum(i => i.qty * i.unitprice);
                if (itemsTotal != request.totalamount)
                {
                    _logger.Warn($"Item total mismatch: Items total = {itemsTotal}, Request total = {request.totalamount}");
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

            var response = new
            {
                result = 1,
                request.totalamount,
                totaldiscount = totalDiscount,
                finalamount = finalAmount
            };

            _logger.Info("Outgoing response: " + JsonSerializer.Serialize(response));
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error("Unexpected error: " + ex.ToString());
            return StatusCode(500, new { result = 0, resultmessage = "Internal Server Error" });
        }
    }

    [HttpPost]
    [Route("v2")]
    public IActionResult Submit2([FromBody] TransactionRequest request)
    {
        _logger.Info("Incoming request: " + JsonSerializer.Serialize(request));

        try
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
                _logger.Warn("Access denied: Invalid partner credentials");
                return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
            }

            if (!SignatureHelper.VerifySignature(request, expectedPassword))
            {
                _logger.Warn("Access denied: Signature mismatch");
                return Unauthorized(new { result = 0, resultmessage = "Access Denied!" });
            }

            if (!TimeHelper.IsTimestampValid(request.timestamp))
            {
                _logger.Warn("Expired request timestamp: " + request.timestamp);
                return BadRequest(new { result = 0, resultmessage = "Expired." });
            }

            if (request.items != null && request.items.Count > 0)
            {
                var itemsTotal = request.items.Sum(i => i.qty * i.unitprice);
                if (itemsTotal != request.totalamount)
                {
                    _logger.Warn($"Item total mismatch: Items total = {itemsTotal}, Request total = {request.totalamount}");
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

            var response = new
            {
                result = 1,
                request.totalamount,
                totaldiscount = totalDiscount,
                finalamount = finalAmount
            };

            _logger.Info("Outgoing response: " + JsonSerializer.Serialize(response));
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error("Unexpected error: " + ex.ToString());
            return StatusCode(500, new { result = 0, resultmessage = "Internal Server Error" });
        }
    }
}