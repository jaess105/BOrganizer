CREATE TABLE invoices
(
    invoice_id                 BIGSERIAL PRIMARY KEY,
    rechnungs_steller_id       BIGINT NOT NULL,
    rechnungs_empfaenger_id    BIGINT NOT NULL,
    steuernummer               TEXT   NOT NULL,
    erstellungs_dateum         DATE   NOT NULL,
    rechnungsnummer_id         BIGINT NOT NULL,
    steuersatz_id              INT    NOT NULL,
    credit_accounts_id         BIGINT NOT NULL,
    angabe_zur_steuerbefreiung TEXT   NOT NULL,

    UStId                      TEXT,
    HRB                        TEXT,
    Amtsgericht                TEXT,
    Geschaeftsfuehrer          TEXT,
    Webseite                   TEXT
);

CREATE TABLE invoice_items
(
    invoice_item_id BIGSERIAL PRIMARY KEY,
    invoice_id      BIGINT         NOT NULL REFERENCES invoices (invoice_id) ON DELETE CASCADE,
    beschreibung    TEXT           NOT NULL,
    quantity        INT            NOT NULL CHECK (quantity >= 0),
    unit_price      NUMERIC(18, 2) NOT NULL CHECK (unit_price >= 0)
);
