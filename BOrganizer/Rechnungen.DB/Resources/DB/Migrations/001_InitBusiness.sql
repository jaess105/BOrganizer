CREATE TABLE persons
(
    id         SERIAL PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name  TEXT NOT NULL,
    UNIQUE (first_name, last_name) -- optional constraint
);

CREATE TABLE businesses
(
    id         SERIAL PRIMARY KEY,
    name       TEXT    NOT NULL,
    street     TEXT    NOT NULL,
    number     TEXT    NOT NULL,
    plz        TEXT    NOT NULL,
    ort        TEXT    NOT NULL,
    country    TEXT,
    tel        TEXT,
    email      TEXT,
    is_primary BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE UNIQUE INDEX unique_primary_business ON businesses (is_primary) WHERE is_primary = TRUE;

CREATE TABLE person_businesses
(
    person_id   INT NOT NULL REFERENCES persons (id) ON DELETE CASCADE,
    business_id INT NOT NULL REFERENCES businesses (id) ON DELETE CASCADE,
    PRIMARY KEY (person_id, business_id)
);




