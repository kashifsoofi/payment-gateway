USE Payments;

CREATE TABLE IF NOT EXISTS Payment (
    ClusteredId     BIGINT          NOT NULL AUTO_INCREMENT,
    Id              CHAR(36)        NOT NULL UNIQUE,
    MerchantId      CHAR(36)        NOT NULL,
    CardHolderName  VARCHAR(255)    NOT NULL,
    CardNumber      CHAR(4)         NOT NULL,
    ExpiryMonth     INT             NOT NULL,
    ExpiryYear      INT             NOT NULL,
    Amount          DECIMAL(10, 4)  NOT NULL,
    CurrencyCode    CHAR(3)         NOT NULL,
    Reference       VARCHAR(50)     NOT NULL,
    Status          VARCHAR(40)     NOT NULL,
    CreatedOn       DATETIME        NOT NULL,
    UpdatedOn       DATETIME        NOT NULL,
    PRIMARY KEY (ClusteredId)
) ENGINE=INNODB;