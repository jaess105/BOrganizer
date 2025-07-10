using Rechnungen.Model.Invoices;

namespace Rechnungen;

public interface IRechnungsCreator
{
    Stream CreateRechnung(Invoice template);
}