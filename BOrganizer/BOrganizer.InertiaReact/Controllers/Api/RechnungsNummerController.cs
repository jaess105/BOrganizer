using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers.Api;

[Route("Api/RechnungsNummer")]
public class RechnungsNummerController(
    IRechnungsService rechnungsService) : Controller
{
    [HttpGet("{rechnungsnummerId:long}")]
    public async Task<IActionResult> Get(long rechnungsnummerId)
    {
        if (await rechnungsService.GetRechnungsNummerByIdAsync(rechnungsnummerId) is { } currentRechnungsNummer)
        {
            return Ok(RechnungsnummerDto.FromDomain(currentRechnungsNummer));
        }

        return NotFound();
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search(string query, long? currentId)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Ok(new List<object>()); }

        IEnumerable<RechnungsNummer> rechnungsNummern = await rechnungsService.SearchRechnungsNummernAsync(query);
        if (currentId is not null &&
            await rechnungsService.GetRechnungsNummerByIdAsync(currentId.Value) is { } currentRechnungsNummer)
        {
            rechnungsNummern = rechnungsNummern.Append(currentRechnungsNummer);
        }

        return Ok(rechnungsNummern.Select(RechnungsnummerDto.FromDomain).ToHashSet());
    }
}

public class RechnungsnummerDto
{
    public required long Id { get; init; }
    public required string Label { get; init; }

    public static RechnungsnummerDto FromDomain(RechnungsNummer rechnungsNummer)
        => new()
        {
            Id = rechnungsNummer.Id!.Value,
            Label = rechnungsNummer.ToString()
        };

    #region Equailty members

    protected bool Equals(RechnungsnummerDto other)
    {
        return Id == other.Id && Label == other.Label;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }

        if (ReferenceEquals(this, obj)) { return true; }

        if (obj.GetType() != GetType()) { return false; }

        return Equals((RechnungsnummerDto)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Label);
    }

    public static bool operator ==(RechnungsnummerDto? left, RechnungsnummerDto? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RechnungsnummerDto? left, RechnungsnummerDto? right)
    {
        return !Equals(left, right);
    }

    #endregion
}