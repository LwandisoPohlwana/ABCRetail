using ABCRetail.AzureTableService.Interfaces;
using ABCRetail.Models;
using ABCRetail.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

public class ShippingDetailsController : Controller
{
    private readonly IShippingDetailService _shippingDetailService;
    private readonly ILogger<ShippingDetailsController> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _functionUrl = "https://abcretailwebfunctions.azurewebsites.net/api/StoreShippingDetails?code=1KlW8jlZwnsQUqfCtWUeGTRkWQHR_i99TPJAteJccmHWAzFuQJUu3Q%3D%3D";

    public ShippingDetailsController(IShippingDetailService shippingDetailService, ILogger<ShippingDetailsController> logger, HttpClient httpClient)
    {
        _shippingDetailService = shippingDetailService;
        _logger = logger;
        _httpClient = httpClient;
    }

    // Add Shipping Detail GET
    public IActionResult AddShippingDetail()
    {
        return View();
    }

    // Add Shipping Detail POST
    [HttpPost]
    public async Task<IActionResult> AddShippingDetail(ShippingDetailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var shippingDetail = new ShippingDetail
            {
                UserId = model.UserId,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country
            };

            try
            {
                // Serialize shipping detail to JSON
                var jsonContent = JsonConvert.SerializeObject(shippingDetail);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Call the Azure Function
                var response = await _httpClient.PostAsync(_functionUrl, contentString);

                // Log response status code and body for debugging
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Shipping details added successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to add shipping details: {response.StatusCode}, {responseBody}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to add shipping details: " + ex.Message;
            }
        }

        return RedirectToAction("ViewShippingDetails");
    }

    // View Shipping Details
    public async Task<IActionResult> ViewShippingDetails()
    {
        // Retrieve the UserId from session or auth context
        var userId = HttpContext.Session.GetString("UserId");

        // Fetch shipping details for the user
        var shippingDetails = await _shippingDetailService.GetShippingDetailsByUserIdAsync(userId);

        // Map to view model if necessary
        var shippingDetailViewModels = shippingDetails.Select(detail => new ShippingDetailViewModel
        {
            AddressLine1 = detail.AddressLine1,
            AddressLine2 = detail.AddressLine2,
            City = detail.City,
            State = detail.State,
            ZipCode = detail.ZipCode,
            Country = detail.Country
        }).ToList();

        return View(shippingDetailViewModels);
    }


    // Delete Shipping Detail
    public async Task<IActionResult> DeleteShippingDetail(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var detail =  _shippingDetailService.DeleteShippingDetailAsync(id, userId); // Assuming this fetches the specific detail by rowKey and userId
        if (detail == null)
        {
            return NotFound();
        }
        return View(detail);
    }

    // Delete Shipping Detail POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteShippingDetailConfirmed(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");

        try
        {
            await _shippingDetailService.DeleteShippingDetailAsync(id, userId); // Delete by rowKey (id) and userId
            TempData["SuccessMessage"] = "Shipping detail successfully deleted.";
        }
        catch (Exception ex)
        {
            // Log the error and display a friendly message to the user
            _logger.LogError(ex, "Error deleting shipping detail.");
            TempData["ErrorMessage"] = "An error occurred while deleting the shipping detail.";
        }

        return RedirectToAction("ViewShippingDetails");
    }
}
