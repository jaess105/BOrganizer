ALTER TABLE invoices
    RENAME COLUMN erstellungs_dateum TO erstellungs_datum;

ALTER TABLE invoices
    ADD COLUMN liefer_datum DATE NOT NULL default NOW();
