CREATE VIEW card AS
SELECT product.id id,product.name name_product,price,is_menu(product.id) menu,product_category.name category_name,product_category.id_store id_store
FROM product 
JOIN product_category ON id_product_category=product_category.id ORDER BY id_store;

CREATE OR REPLACE VIEW menu AS
SELECT p1.id id,p2.id id_menu,quantity,p2.price price,p2.name menu_name,p1.name product_name,product_category.id_store id_store,p2.id_product_category id_product_category
FROM product p1
JOIN contain ON id=id_product
JOIN product p2 ON contain.id_menu=p2.id
JOIN product_category ON p2.id_product_category=product_category.id WHERE (SELECT is_menu(id_menu));

CREATE OR REPLACE VIEW category AS
SELECT product.id id,product_category.name category_name,product.name as name,price,product_category.id id_category,id_store
FROM product
JOIN product_category ON product_category.id=id_product_category;


CREATE VIEW fullsellout AS 
SELECT id_person,receipt_date,livraison_date,id_product,quantity,sellout.id id,id_store 
FROM sellout 
JOIN concern ON id_product=id 
JOIN product ON product.id=id_product
JOIN product_category ON product.id_product_category=product_category.id ORDER BY sellout.id;

CREATE OR REPLACE VIEW full_store AS
SELECT person.id id,town,adress,logo_path,name,surname,store.id id_store
FROM store
JOIN person ON id_person=person.id;

CREATE OR REPLACE VIEW full_user AS
SELECT person.id id,mail,name,surname,pseudo,password,id_store,_user.id id_user,id_role
FROM _user 
JOIN person ON id_person=person.id;

CREATE OR REPLACE VIEW client AS
SELECT person.id id,mail,name,surname,store.id id_store
FROM person
JOIN buy ON buy.id_person=person.id
JOIN store ON buy.id_store=store.id;

