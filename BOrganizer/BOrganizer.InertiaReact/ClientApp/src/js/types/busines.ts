export interface Business {
    id: number;
    name: string;
    street: string;
    number: string;
    plz: string;
    ort: string;
    tel?: string;
    eMail?: string;
    country?: string;
    steuernummer?: string;
}

interface RechnungsNummer {
    Id?: number | null;
    Kuerzel: string;
    Jahr: string;
    Nummer: string;

    toString(): string;
}

enum InvoiceSteuersatzId {
    Standard = 1,
    Ermaessigt = 2,
}

class InvoiceSteuersatz {
    static readonly Standard = new InvoiceSteuersatz("Standard", 0.19, InvoiceSteuersatzId.Standard);
    static readonly Ermaessigt = new InvoiceSteuersatz("Ermäßigt", 0.07, InvoiceSteuersatzId.Ermaessigt);

    static readonly Steuersaetze = [InvoiceSteuersatz.Standard, InvoiceSteuersatz.Ermaessigt];

    readonly Id: InvoiceSteuersatzId;
    readonly SteuerSatz: string;
    readonly InProzent: number;

    private constructor(steuerSatz: string, inProzent: number, id: InvoiceSteuersatzId) {
        this.SteuerSatz = steuerSatz;
        this.InProzent = inProzent;
        this.Id = id;
    }

    static ById(id: InvoiceSteuersatzId | number): InvoiceSteuersatz {
        switch (id) {
            case InvoiceSteuersatzId.Standard:
                return InvoiceSteuersatz.Standard;
            case InvoiceSteuersatzId.Ermaessigt:
                return InvoiceSteuersatz.Ermaessigt;
            default:
                throw new RangeError(`Invalid InvoiceSteuersatzId: ${id}`);
        }
    }

    toString(): string {
        return `${this.SteuerSatz} (${(this.InProzent * 100).toFixed(0)}%)`;
    }

    equals(other: InvoiceSteuersatz): boolean {
        return this.SteuerSatz === other.SteuerSatz && this.InProzent === other.InProzent;
    }
}

interface Credit {
    Id?: number | null;
    Institute: string;
    IBAN: string;
    BIC: string;
    Inhaber: string;

    short(): string;

    ibanNotation(): string;
}

class CreditImpl implements Credit {
    Id?: number | null;
    Institute: string;
    IBAN: string;
    BIC: string;
    Inhaber: string;

    constructor(Institute: string, IBAN: string, BIC: string, Inhaber: string, Id?: number | null) {
        this.Id = Id ?? null;
        this.Institute = Institute;
        this.IBAN = IBAN;
        this.BIC = BIC;
        this.Inhaber = Inhaber;
    }

    short(): string {
        return `${this.Institute}: ${this.IBAN}`;
    }

    ibanNotation(): string {
        const together = this.IBAN.replace(/\s+/g, "");
        return Array.from({length: Math.ceil(together.length / 4)}, (_, i) =>
            together.substr(i * 4, 4)
        ).join(" ");
    }
}


interface InvoiceItem {
    beschreibung: string;
    quantity: number; // uint in C# → number (assumed non-negative)
    unitPrice: number;
    hardTotal?: number | null;
    total: number;
    rechnungsbetrag: number;
}

export interface Invoice {
    rechnungsSteller: Business;
    rechnungsEmpfaenger: Business;
    erstellungsDatum: string; // DateOnly → ISO date string "YYYY-MM-DD"
    lieferDatum: string;      // DateOnly → ISO date string
    rechnungsnummer: RechnungsNummer;
    steuerAusweisung: InvoiceSteuersatz;
    zahlungsziel: string;
    angabeZurSteuerbefreiung: string;
    items: InvoiceItem[];
    rechnungsStellerCredit: Credit;
    uStId?: string | null;
    hrb?: string | null;
    amtsgericht?: string | null;
    geschaeftsfuehrer?: string | null;
    webseite?: string | null;

    id?: number | null;
    gesamtBetrag: number;
}