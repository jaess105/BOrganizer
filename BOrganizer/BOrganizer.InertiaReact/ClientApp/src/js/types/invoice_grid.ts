type Business = {
    name: string;
};

type Invoice = {
    id: string;
    rechnungsnummer: string;
    erstellungsDatum: string;
    rechnungsSteller: Business;
    rechnungsEmpfaenger: Business;
    gesamtBetrag: number;
};