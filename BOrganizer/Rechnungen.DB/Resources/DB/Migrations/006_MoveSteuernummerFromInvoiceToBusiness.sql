ALTER TABLE invoices.public.invoices
    DROP COLUMN steuernummer;

ALTER TABLE invoices.public.businesses
    ADD COLUMN steuernummer text;
