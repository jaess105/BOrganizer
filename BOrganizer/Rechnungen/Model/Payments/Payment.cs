namespace Rechnungen.Model.Payments;

public record Payment(
    DateOnly Date,
    decimal MwstPercent,
    decimal Netto,
    decimal Brutto,
    decimal MwstTotal,
    string Sender,
    string Receiver,
    string Product,
    string Currency,
    string? Method = null,
    string? Notes = null,
    long? SenderId = null,
    long? ReceiverId = null,
    long? RechnungId = null
)
{
    public required long? Id { get; init; }
}