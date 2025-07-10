CREATE TABLE credit_accounts
(
    id        SERIAL PRIMARY KEY,
    institute TEXT NOT NULL,
    iban      TEXT NOT NULL,
    bic       TEXT NOT NULL,
    inhaber   TEXT NOT NULL
);