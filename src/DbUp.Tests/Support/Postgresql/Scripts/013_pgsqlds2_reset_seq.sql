
-- pgsqlds2_create_db.sql: DVD Store Database Version 2.1 Build Script - Postgres version
-- Copyright (C) 2011 Vmware, Inc. 
-- Last updated 2/13/11


-- Reset Sequences after load

SELECT setval('categories_category_seq',max(category)) FROM categories;
SELECT setval('customers_customerid_seq',max(customerid)) FROM customers;
SELECT setval('orders_orderid_seq',max(orderid)) FROM orders;
SELECT setval('products_prod_id_seq',max(prod_id)) FROM products;

