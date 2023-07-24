DROP TABLE IF EXISTS contain;
DROP TABLE IF EXISTS concern;
DROP TABLE IF EXISTS _user;
DROP TABLE IF EXISTS mail_parameter;
DROP TABLE IF EXISTS bill_parameter;
DROP TABLE IF EXISTS product;
DROP TABLE IF EXISTS product_category;
DROP TABLE IF EXISTS store;
DROP TABLE IF EXISTS sellout;
DROP TABLE IF EXISTS person;
DROP TABLE IF EXISTS store_chain;
DROP TABLE IF EXISTS role;

CREATE TABLE role (
   id INT AUTO_INCREMENT,
   name VARCHAR(50) NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (name)
);

CREATE TABLE store_chain (
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (name)
);

CREATE TABLE person (
   id INT AUTO_INCREMENT,
   mail VARCHAR(255) NOT NULL,
   name VARCHAR(255) NOT NULL,
   surname VARCHAR(255),
   id_store_chain INTEGER,
   PRIMARY KEY (id),
   UNIQUE (id_store_chain, mail),
   FOREIGN KEY (id_store_chain) REFERENCES store_chain (id) ON DELETE CASCADE
);

CREATE TABLE sellout (
   id INT AUTO_INCREMENT,
   receipt_date DATE NOT NULL,
   livraison_date DATE NOT NULL,
   id_person INTEGER NOT NULL,
   PRIMARY KEY (id),
   FOREIGN KEY (id_person) REFERENCES person (id) ON DELETE CASCADE
);

CREATE TABLE store (
   id INT AUTO_INCREMENT,
   town VARCHAR(255) NOT NULL,
   adress VARCHAR(255) NOT NULL,
   logo_path VARCHAR(255) NOT NULL,
   id_store_chain INTEGER ,
   id_person INTEGER NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (id_person),
   UNIQUE (logo_path),
   FOREIGN KEY (id_store_chain) REFERENCES store_chain (id) ON DELETE CASCADE,
   FOREIGN KEY (id_person) REFERENCES person (id) ON DELETE CASCADE
);

CREATE TABLE event(
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   start DATE NOT NULL,
   end DATE NOT NULL,
   id_store INT NOT NULL,
   PRIMARY KEY (id),
   FOREIGN KEY (id_store) REFERENCES store(id)
)

CREATE TABLE product_category (
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   id_store INTEGER NOT NULL,
   id_container INTEGER,
   image_path VARCHAR(255) UNIQUE,
   PRIMARY KEY (id),
   FOREIGN KEY (id_store) REFERENCES store (id),
   FOREIGN KEY (id_container) REFERENCES product_category (id) ON DELETE CASCADE
);

CREATE TABLE product (
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   price INTEGER,
   id_product_category INTEGER NOT NULL,
   image_path VARCHAR(255) UNIQUE,
   PRIMARY KEY (id),
   FOREIGN KEY (id_product_category) REFERENCES product_category (id) ON DELETE CASCADE
);

CREATE TABLE bill_parameter (
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   foot VARCHAR(255) NOT NULL DEFAULT 'placeholder',
   special_mention VARCHAR(255),
   mention VARCHAR(255),
   id_store INTEGER NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (name, id_store),
   UNIQUE (foot),
   FOREIGN KEY (id_store) REFERENCES store (id) ON DELETE CASCADE
);

CREATE TABLE mail_content_parameter(
   id INT AUTO_INCREMENT,
   name VARCHAR(255) NOT NULL,
   object VARCHAR(255) NOT NULL,
   mail_head TEXT NOT NULL,
   mail_foot TEXT NOT NULL,
   id_store INT NOT NULL,
   FOREIGN KEY (id_store) REFERENCES store(id)
)

CREATE TABLE buy (
   id_store INTEGER,
   id_person INTEGER,
   PRIMARY KEY (id_store, id_person),
   FOREIGN KEY (id_store) REFERENCES store (id) ON DELETE CASCADE,
   FOREIGN KEY (id_person) REFERENCES person (id) ON DELETE CASCADE
);

CREATE TABLE mail_parameter (
   id INT AUTO_INCREMENT,
   connection_type VARCHAR(255) NOT NULL,
   server VARCHAR(255) NOT NULL,
   password VARCHAR(255) NOT NULL,
   port INT NOT NULL,
   id_store INTEGER NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (login),
   FOREIGN KEY (id_store) REFERENCES store (id) ON DELETE CASCADE
);

CREATE TABLE _user (
   id INT AUTO_INCREMENT,
   pseudo VARCHAR(20) NOT NULL,
   password VARCHAR(255) NOT NULL,
   id_person INTEGER NOT NULL,
   id_store INTEGER,
   id_role INTEGER NOT NULL,
   PRIMARY KEY (id),
   UNIQUE (id_person),
   UNIQUE (pseudo),
   UNIQUE (password),
   FOREIGN KEY (id_person) REFERENCES person (id) ON DELETE CASCADE,
   FOREIGN KEY (id_store) REFERENCES store (id) ON DELETE CASCADE,
   FOREIGN KEY (id_role) REFERENCES role (id) ON DELETE CASCADE
);

CREATE TABLE concern (
   id_product INTEGER,
   id_sellout INTEGER,
   quantity INT UNSIGNED NOT NULL,
   PRIMARY KEY (id_product, id_sellout),
   FOREIGN KEY (id_product) REFERENCES product (id) ON DELETE CASCADE,
   FOREIGN KEY (id_sellout) REFERENCES sellout (id) ON DELETE CASCADE
);

CREATE TABLE contain (
   id_product INTEGER,
   id_menu INTEGER,
   quantity INT UNSIGNED NOT NULL,
   PRIMARY KEY (id_product, id_menu),
   FOREIGN KEY (id_product) REFERENCES product (id) ON DELETE CASCADE,
   FOREIGN KEY (id_menu) REFERENCES product (id) ON DELETE CASCADE
);
