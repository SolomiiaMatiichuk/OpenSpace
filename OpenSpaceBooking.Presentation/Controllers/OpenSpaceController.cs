using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenSpaceBooking.BLL.Dtos;
using OpenSpaceBooking.BLL.Interfaces;

namespace OpenSpaceBooking.Presentation.Controllers;

public class OpenSpaceController : BaseController
{
    private readonly IOpenSpaceService _openSpaceService;
    private readonly IReservationService _reservationService;

    public OpenSpaceController(IOpenSpaceService photoZoneService, IReservationService reservationService)
    {
        _openSpaceService = photoZoneService;
        _reservationService = reservationService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> OpenSpaceWithDetail(int id, string? query)
    {
        var field = await _openSpaceService.GetOpenSpaceWithDetailsAsync(id);
        var reservations = await _reservationService.GetOpenSpaceReservationsByTitleAsync(query, id);
        ViewBag.SearchedReservations = reservations;
        return View(field);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var photoZones = await _openSpaceService.GetOpenSpacesWithDetailsAsync();
        return View(photoZones);
    }
    
    public IActionResult PostOpenSpace()
    {
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> PostOpenSpace(OpenSpaceDto model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _openSpaceService.CreateOpenSpace(model);
        if (result.StatusCode != HttpStatusCode.BadRequest) return RedirectToAction("Index");
        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }
    
    public async Task<IActionResult> DeleteOpenSpace(int id)
    {
        await _openSpaceService.DeleteOpenSpace(id);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> UpdateOpenSpace(int id)
    {
        var photoZone = await _openSpaceService.GetOpenSpaceWithDetailsAsync(id);
        return View(photoZone);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateOpenSpace(OpenSpaceDto model)
    {
        if (!ModelState.IsValid) return View(model);
        await _openSpaceService.UpdateOpenSpace(model);
        return RedirectToAction("Index");
    }
}