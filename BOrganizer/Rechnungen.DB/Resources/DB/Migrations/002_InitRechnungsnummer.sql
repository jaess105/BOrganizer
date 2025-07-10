CREATE TABLE rechnungsnummer
(
    kuerzel    TEXT      NOT NULL,
    year       TEXT      NOT NULL,
    number     TEXT      NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT now(),

    PRIMARY KEY (kuerzel, year, number),
    CHECK (number ~ '^\d+$') -- Ensures number is only digits (e.g., "001", "002")
);
