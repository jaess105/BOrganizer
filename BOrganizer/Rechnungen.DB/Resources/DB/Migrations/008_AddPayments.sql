CREATE TABLE payments
(
    payment_id           BIGSERIAL PRIMARY KEY,
    date         DATE           NOT NULL,
    mwst_percent NUMERIC(5, 2)  NOT NULL,
    netto        NUMERIC(12, 2) NOT NULL,
    brutto       NUMERIC(12, 2) NOT NULL,
    mwst_total NUMERIC(12, 2) NOT NULL,
    sender       TEXT           NOT NULL,
    receiver     TEXT           NOT NULL,
    product      TEXT           NOT NULL,
    currency     VARCHAR(10)    NOT NULL,
    method       VARCHAR(50), -- Enum as text (e.g. 'BankTransfer')
    notes        TEXT,
    sender_id    BIGINT         REFERENCES businesses (id) ON DELETE SET NULL,
    receiver_id  BIGINT         REFERENCES businesses (id) ON DELETE SET NULL,
    rechnung_id  BIGINT         REFERENCES invoices (invoice_id) ON DELETE SET NULL
);
