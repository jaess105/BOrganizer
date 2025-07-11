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

    Short(): string;

    IbanNotation(): string;
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

    Short(): string {
        return `${this.Institute}: ${this.IBAN}`;
    }

    IbanNotation(): string {
        const together = this.IBAN.replace(/\s+/g, "");
        return Array.from({length: Math.ceil(together.length / 4)}, (_, i) =>
            together.substr(i * 4, 4)
        ).join(" ");
    }
}


interface InvoiceItem {
    Beschreibung: string;
    Quantity: number; // uint in C# → number (assumed non-negative)
    UnitPrice: number;
    HardTotal?: number | null;
    Total: number;
    Rechnungsbetrag: number;
}

export interface Invoice {
    RechnungsSteller: Business;
    RechnungsEmpfaenger: Business;
    ErstellungsDatum: string; // DateOnly → ISO date string "YYYY-MM-DD"
    LieferDatum: string;      // DateOnly → ISO date string
    Rechnungsnummer: RechnungsNummer;
    SteuerAusweisung: InvoiceSteuersatz;
    Zahlungsziel: string;
    AngabeZurSteuerbefreiung: string;
    Items: InvoiceItem[];
    RechnungsStellerCredit: Credit;
    UStId?: string | null;
    HRB?: string | null;
    Amtsgericht?: string | null;
    Geschaeftsfuehrer?: string | null;
    Webseite?: string | null;

    id?: number | null;
    GesamtBetrag: number;
}