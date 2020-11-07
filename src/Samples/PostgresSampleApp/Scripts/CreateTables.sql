
/**
  Fairly unoriginal book/author example database.
  This isn't the fun part!
 */

create table "Authors" (
    "Id" serial not null,
    "Name" text not null,
    constraint PK_Authors primary key ("Id")
);

create table "Books" (
    "Id" serial not null,
    "Name" text not null,
    "AuthorId" int not null,
    constraint PK_Books primary key ("Id"),
    constraint FK_Book_Author foreign key ("AuthorId") references "Authors" ("Id")
);