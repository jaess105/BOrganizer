ALTER TABLE rechnungsnummer
    ADD COLUMN id BIGSERIAL;

ALTER TABLE rechnungsnummer
    DROP CONSTRAINT rechnungsnummer_pkey;

ALTER TABLE rechnungsnummer
    ADD CONSTRAINT rechnungsnummer_pkey PRIMARY KEY (id);

ALTER TABLE rechnungsnummer
    ADD CONSTRAINT rechnungsnummer_unique UNIQUE (kuerzel, year, number);
