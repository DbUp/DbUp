# How to deploy a database
*This post was originally posted on [Paul Stovell's blog](http://paulstovell.com/blog/database-deployment) under the same title as this page. It has been copied to the DbUp docs so it is with the rest of the documentation and can be evolved over time.*

When I'm building an application that stores data, there are a few things I try to make sure we can do when it comes to managing the lifecycle of our storage. I'm going to focus on SQL Server, but this applies to other relational storage.

# What's in a database?
Even outside of production, a database is more than schema. A database "definition" consists of:

* The database schema (tables, views, procedures, schemas)
* [Reference data](http://en.wikipedia.org/wiki/Reference_data) (ZIP codes, salutations, other things I expect to always "be there")
* Sample master/transactional data (for developers/testers)
* Security (roles, permissions, users)

It may even include management job definitions (backup, shrink), though they tend to be left to DBA's.

# Transitions, not States
As each iteration progresses, we'll make changes to our database.

One approach to upgrading existing databases is to "diff" the old state of the database with our new state, and then to manually modify this diff until we have something that works. We then employ the "hope and pray" strategy of managing our database, which generally means "leave it to the DBA" for production. Tools like [Red Gate SQL Compare](http://www.red-gate.com/products/SQL_Compare/) and [Visual Studio Database Edition](http://blogs.msdn.com/b/gertd/archive/2008/11/25/visual-studio-team-system-2008-database-edition-gdr-rtm.aspx) encourage this approach.

I never understood the state or model-driven approach, and I'll explain why. Here's a simple example of some T-SQL:

```sql
create table dbo.Customer (
    Id int not null identity(1,1) constraint PK_CustomerId primary key,
    FullName nvarchar(200) not null
);
```

See the "create table"? That's not a definition - it's an instruction. If I need to change my table, I don't change the statement above. I just write:

``` sql
alter table dbo.Customer
    add IsPreferred bit not null
        constraint DF_Customer_IsPreferred default(0);
```

Again, an instruction. A transition. I don't tell the database what the state is - I tell it how to get there. I can provide a lot more context, and I have a lot more power.

You'll note that I used a default constraint above - that's because my table might have had data already. Since I was thinking about transitions, I was forced to think about these issues.

Our databases are designed for transitions; attempting to bolt a state-based approach on them is about as dumb as [bolting a stateful model on top of the web](http://weblogs.asp.net/infinitiesloop/archive/2006/08/03/Truly-Understanding-Viewstate.aspx) (and Visual Studio Database Edition is about as much fun as ViewState).

Keep in mind that making changes to databases can be complicated. Here are some things we might do:

* Add a column to an existing table (what will the default values be?)
* Split a column into two columns (how will you deal with data in the existing column?)
* Move a column from one table onto another (remember to move it, not to drop and create the column and lose the data)
* Duplicate data from a column on one table into a column on another (to reduce joins) (don't just create the empty column - figure out how to get the data there)
* Rename a column (don't just create a new one and delete the old one)
* Change the type of a column (how will you convert the old data? What if some rows won't convert?)
* Change the data within a column (maybe all order #'s need to be prefixed with the customer code?)

You can see how performing a "diff" on the old and new state can miss some of the intricacies of real-life data management.

# Successful database management
Here are some things I want from my database deployment strategy:

1. **Source controlled**  
    Your database isn't in source control? You don't deserve one. Go use Excel.
2. **Testability**  
    I want to be able to write an integration test that takes a backup of the old state, performs the upgrade to the current state, and verifies that the data wasn't corrupted.
3.  **Continuous integration**  
    I want those tests run by my build server, every time I check in. I'd like a CI build that takes a production backup, restores it, and runs and tests any upgrades nightly.
4. **No shared databases**  
    Every developer should be able to have a copy of the database on their own machine. Deploying that database - with sample data - should be one click.
5. **Dogfooding upgrades**  
    If Susan makes a change to the database, Harry should be able to execute her transitions on his own database. If he had different test data to her, he might find bugs she didn't. Harry shouldn't just blow away his database and start again.

The benefits to this are enormous. By testing my transitions **every single day** - on my own dev test data, in my integration tests, on my build server, against production backups - I'm going to be confident that my changes will work in production.

# Versions table
There should be something that I can query to know which transitions have been run against my database. The simplest way is with a `Versions` table, which tells me the scripts that were run, when they were run, and who they were run by.

When it comes to upgrading, I can query this table, skip the transitions that have been run, and execute the ones that haven't.

# Sample data
Development teams often need access to a good set of sample data, ideally lots of it. Again, these should be transition scripts that can be optionally run (since I might not want sample data in production), and in source control.

# Document Databases
Most of these principles apply to document databases too. In fact, in some ways the problems are harder. While you don't have a fixed schema, you're probably mapping your documents to objects - what if the structure of the objects change? You _may_ need to run transitional scripts over the document database to manipulate the existing documents. You may also need to re-define your indexes. You want those deployment scenarios to be testable and trusted.

# Migration libraries
Rails popularized the approach of [using a DSL to describe data migrations](http://guides.rubyonrails.org/migrations.html). There are a number of .NET ports of this concept, like [Fluent Migrator](https://github.com/schambers/fluentmigrator) and [Machine.Migrations](http://blog.eleutian.com/2008/04/25/AFirstLookAtMachineMigrations.aspx).

Personally, I actually find T-SQL a perfectly good DSL for describing data migrations. I love my ORM's, but for schema work, T-SQL is perfectly adequate.

Again, these libraries focus on transitions (create table, add column), not states, so they're useful, unlike Visual Studio Database Edition, which isn't.
