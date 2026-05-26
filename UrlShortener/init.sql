CREATE TABLE urls (
    id          BIGSERIAL PRIMARY KEY,
    short_key   VARCHAR(10)  NOT NULL UNIQUE,
    original_url TEXT        NOT NULL,
    created_at  TIMESTAMPTZ  DEFAULT NOW(),
    expires_at  TIMESTAMPTZ  NULL,
    is_active   BOOLEAN      DEFAULT TRUE
);

CREATE INDEX idx_short_key ON urls(short_key);