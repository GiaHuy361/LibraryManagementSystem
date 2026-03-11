IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [authors] (
    [author_id] int NOT NULL IDENTITY,
    [author_name] nvarchar(150) NOT NULL,
    [biography] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_authors] PRIMARY KEY ([author_id])
);
GO

CREATE TABLE [categories] (
    [category_id] int NOT NULL IDENTITY,
    [category_name] nvarchar(100) NOT NULL,
    [description] nvarchar(255) NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY ([category_id])
);
GO

CREATE TABLE [roles] (
    [role_id] int NOT NULL IDENTITY,
    [role_name] nvarchar(50) NOT NULL,
    [description] nvarchar(255) NULL,
    CONSTRAINT [PK_roles] PRIMARY KEY ([role_id])
);
GO

CREATE TABLE [books] (
    [book_id] int NOT NULL IDENTITY,
    [title] nvarchar(255) NOT NULL,
    [isbn] nvarchar(20) NULL,
    [category_id] int NULL,
    [publisher] nvarchar(150) NULL,
    [publish_year] int NULL,
    [description] NVARCHAR(MAX) NULL,
    [quantity] int NOT NULL DEFAULT 1,
    [status] nvarchar(20) NOT NULL DEFAULT N'Available',
    [created_at] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_books] PRIMARY KEY ([book_id]),
    CONSTRAINT [FK_books_categories_category_id] FOREIGN KEY ([category_id]) REFERENCES [categories] ([category_id]) ON DELETE SET NULL
);
GO

CREATE TABLE [users] (
    [user_id] int NOT NULL IDENTITY,
    [username] nvarchar(50) NOT NULL,
    [password_hash] nvarchar(255) NOT NULL,
    [full_name] nvarchar(100) NULL,
    [email] nvarchar(100) NULL,
    [phone] nvarchar(20) NULL,
    [role_id] int NULL,
    [status] nvarchar(20) NOT NULL DEFAULT N'Active',
    [created_at] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_users] PRIMARY KEY ([user_id]),
    CONSTRAINT [FK_users_roles_role_id] FOREIGN KEY ([role_id]) REFERENCES [roles] ([role_id]) ON DELETE SET NULL
);
GO

CREATE TABLE [book_authors] (
    [book_id] int NOT NULL,
    [author_id] int NOT NULL,
    CONSTRAINT [PK_book_authors] PRIMARY KEY ([book_id], [author_id]),
    CONSTRAINT [FK_book_authors_authors_author_id] FOREIGN KEY ([author_id]) REFERENCES [authors] ([author_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_book_authors_books_book_id] FOREIGN KEY ([book_id]) REFERENCES [books] ([book_id]) ON DELETE CASCADE
);
GO

CREATE TABLE [borrow_records] (
    [borrow_id] int NOT NULL IDENTITY,
    [member_id] int NOT NULL,
    [librarian_id] int NULL,
    [borrow_date] date NOT NULL,
    [due_date] date NOT NULL,
    [return_date] date NULL,
    [status] nvarchar(20) NOT NULL DEFAULT N'Borrowing',
    [total_fine] decimal(10,2) NOT NULL DEFAULT 0.0,
    CONSTRAINT [PK_borrow_records] PRIMARY KEY ([borrow_id]),
    CONSTRAINT [FK_borrow_records_users_librarian_id] FOREIGN KEY ([librarian_id]) REFERENCES [users] ([user_id]) ON DELETE SET NULL,
    CONSTRAINT [FK_borrow_records_users_member_id] FOREIGN KEY ([member_id]) REFERENCES [users] ([user_id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [borrow_details] (
    [borrow_detail_id] int NOT NULL IDENTITY,
    [borrow_id] int NULL,
    [book_id] int NULL,
    [quantity] int NOT NULL DEFAULT 1,
    [return_date] date NULL,
    [status] nvarchar(20) NOT NULL DEFAULT N'Borrowed',
    CONSTRAINT [PK_borrow_details] PRIMARY KEY ([borrow_detail_id]),
    CONSTRAINT [FK_borrow_details_books_book_id] FOREIGN KEY ([book_id]) REFERENCES [books] ([book_id]) ON DELETE SET NULL,
    CONSTRAINT [FK_borrow_details_borrow_records_borrow_id] FOREIGN KEY ([borrow_id]) REFERENCES [borrow_records] ([borrow_id]) ON DELETE CASCADE
);
GO

CREATE TABLE [fines] (
    [fine_id] int NOT NULL IDENTITY,
    [borrow_detail_id] int NULL,
    [days_overdue] int NOT NULL,
    [fine_amount] decimal(10,2) NOT NULL,
    [paid_status] bit NOT NULL DEFAULT CAST(0 AS bit),
    [paid_date] date NULL,
    CONSTRAINT [PK_fines] PRIMARY KEY ([fine_id]),
    CONSTRAINT [FK_fines_borrow_details_borrow_detail_id] FOREIGN KEY ([borrow_detail_id]) REFERENCES [borrow_details] ([borrow_detail_id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'role_id', N'description', N'role_name') AND [object_id] = OBJECT_ID(N'[roles]'))
    SET IDENTITY_INSERT [roles] ON;
INSERT INTO [roles] ([role_id], [description], [role_name])
VALUES (1, N'System administrator', N'Admin'),
(2, N'Library staff', N'Librarian'),
(3, N'Library member', N'Member');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'role_id', N'description', N'role_name') AND [object_id] = OBJECT_ID(N'[roles]'))
    SET IDENTITY_INSERT [roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'full_name', N'password_hash', N'phone', N'role_id', N'status', N'username') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] ON;
INSERT INTO [users] ([user_id], [created_at], [email], [full_name], [password_hash], [phone], [role_id], [status], [username])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'admin@lms.com', N'System Administrator', N'$2a$11$CjFWpqhB/5WJkQrdWPhXiOBCCL4X1RDt8cqIvn.p2cpCxIlWuPMpC', NULL, 1, N'Active', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'user_id', N'created_at', N'email', N'full_name', N'password_hash', N'phone', N'role_id', N'status', N'username') AND [object_id] = OBJECT_ID(N'[users]'))
    SET IDENTITY_INSERT [users] OFF;
GO

CREATE INDEX [IX_book_authors_author_id] ON [book_authors] ([author_id]);
GO

CREATE INDEX [IX_books_category_id] ON [books] ([category_id]);
GO

CREATE UNIQUE INDEX [IX_books_isbn] ON [books] ([isbn]) WHERE [isbn] IS NOT NULL;
GO

CREATE INDEX [IX_borrow_details_book_id] ON [borrow_details] ([book_id]);
GO

CREATE INDEX [IX_borrow_details_borrow_id] ON [borrow_details] ([borrow_id]);
GO

CREATE INDEX [IX_borrow_records_librarian_id] ON [borrow_records] ([librarian_id]);
GO

CREATE INDEX [IX_borrow_records_member_id] ON [borrow_records] ([member_id]);
GO

CREATE UNIQUE INDEX [IX_fines_borrow_detail_id] ON [fines] ([borrow_detail_id]) WHERE [borrow_detail_id] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_users_email] ON [users] ([email]) WHERE [email] IS NOT NULL;
GO

CREATE INDEX [IX_users_role_id] ON [users] ([role_id]);
GO

CREATE UNIQUE INDEX [IX_users_username] ON [users] ([username]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260311172843_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'author_id', N'author_name', N'biography') AND [object_id] = OBJECT_ID(N'[authors]'))
    SET IDENTITY_INSERT [authors] ON;
INSERT INTO [authors] ([author_id], [author_name], [biography])
VALUES (1, N'Robert C. Martin', N'Uncle Bob, software engineer and author'),
(2, N'Martin Fowler', N'Software developer and author'),
(3, N'Frank Herbert', N'American science fiction writer');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'author_id', N'author_name', N'biography') AND [object_id] = OBJECT_ID(N'[authors]'))
    SET IDENTITY_INSERT [authors] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'category_id', N'category_name', N'description') AND [object_id] = OBJECT_ID(N'[categories]'))
    SET IDENTITY_INSERT [categories] ON;
INSERT INTO [categories] ([category_id], [category_name], [description])
VALUES (1, N'Information Technology', N'Programming, Networking, and Systems'),
(2, N'Science Fiction', N'Sci-Fi novels and literature'),
(3, N'Business', N'Management, Finance, and Economics');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'category_id', N'category_name', N'description') AND [object_id] = OBJECT_ID(N'[categories]'))
    SET IDENTITY_INSERT [categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'book_id', N'category_id', N'created_at', N'description', N'isbn', N'publish_year', N'publisher', N'quantity', N'status', N'title') AND [object_id] = OBJECT_ID(N'[books]'))
    SET IDENTITY_INSERT [books] ON;
INSERT INTO [books] ([book_id], [category_id], [created_at], [description], [isbn], [publish_year], [publisher], [quantity], [status], [title])
VALUES (1, 1, '2026-01-01T00:00:00.0000000Z', NULL, N'9780132350884', 2008, N'Prentice Hall', 5, N'Available', N'Clean Code: A Handbook of Agile Software Craftsmanship'),
(2, 1, '2026-01-01T00:00:00.0000000Z', NULL, N'9780201485677', 1999, N'Addison-Wesley', 3, N'Available', N'Refactoring: Improving the Design of Existing Code'),
(3, 2, '2026-01-01T00:00:00.0000000Z', NULL, N'9780441172719', 1965, N'Chilton Books', 10, N'Available', N'Dune');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'book_id', N'category_id', N'created_at', N'description', N'isbn', N'publish_year', N'publisher', N'quantity', N'status', N'title') AND [object_id] = OBJECT_ID(N'[books]'))
    SET IDENTITY_INSERT [books] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'author_id', N'book_id') AND [object_id] = OBJECT_ID(N'[book_authors]'))
    SET IDENTITY_INSERT [book_authors] ON;
INSERT INTO [book_authors] ([author_id], [book_id])
VALUES (1, 1),
(2, 2),
(3, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'author_id', N'book_id') AND [object_id] = OBJECT_ID(N'[book_authors]'))
    SET IDENTITY_INSERT [book_authors] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260311181113_SeedInitialData', N'8.0.0');
GO

COMMIT;
GO

