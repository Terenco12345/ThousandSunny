CREATE TABLE "User"(
	"Id" UNIQUEIDENTIFIER PRIMARY KEY,
	"DisplayName" varchar(32) NOT NULL,
	"Email" varchar(255) NOT NULL UNIQUE,
	"Password" varchar(64) NOT NULL,
	"CreationDate" datetime NOT NULL,
	"IsActive" bit NOT NULL
)