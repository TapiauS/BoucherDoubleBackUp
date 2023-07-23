CREATE VIEW card AS
SELECT
    product.id AS id,
    product.name AS name_product,
    price,
    is_menu(product.id) AS menu,
    product_category.name AS category_name,
    product_category.id_store AS id_store
FROM
    product
    JOIN product_category ON product.id_product_category = product_category.id
ORDER BY
    id_store;

CREATE VIEW menu AS
SELECT
    p1.id AS id,
    p2.id AS id_menu,
    quantity,
    p2.price AS price,
    p2.name AS menu_name,
    p1.name AS product_name,
    product_category.id_store AS id_store,
    p2.id_product_category
FROM
    product p1
    JOIN contain ON p1.id = contain.id_product
    JOIN product p2 ON contain.id_menu = p2.id
    JOIN product_category ON p2.id_product_category = product_category.id
ORDER BY
    id_menu;

CREATE OR REPLACE VIEW category AS
SELECT
    product.id AS id,
    product_category.name AS category_name,
    product.name AS name,
    price,
    product_category.id AS id_category,
    id_store,
    product.picture_path picture_path
FROM
    product
    JOIN product_category ON product_category.id = product.id_product_category;

CREATE VIEW fullsellout AS
SELECT
    sellout.id_person,
    receipt_date,
    livraison_date,
    id_product,
    quantity,
    sellout.id AS id,
    id_store
FROM
    sellout
    JOIN concern ON sellout.id = concern.id_sellout
    JOIN product ON product.id = concern.id_product
    JOIN product_category ON product.id_product_category = product_category.id
ORDER BY
    sellout.id;

CREATE OR REPLACE VIEW full_store AS
SELECT
    person.id AS id,
    town,
    adress,
    logo_path,
    name,
    surname,
    mail,
    phone_number,
    store.id AS id_store
FROM
    store
    JOIN person ON store.id_person = person.id;

CREATE OR REPLACE VIEW full_user AS
SELECT
    person.id AS id,
    mail,
    name,
    surname,
    pseudo,
    password,
    id_store,
    _user.id AS id_user,
    id_role
FROM
    _user
    JOIN person ON _user.id_person = person.id;

CREATE OR REPLACE VIEW client AS
SELECT
    person.id AS id,
    mail,
    name,
    surname,
    store.id AS id_store,
    phone_number
FROM
    person
    JOIN buy ON buy.id_person = person.id
    JOIN store ON buy.id_store = store.id;