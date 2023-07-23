DROP TABLE IF EXISTS contain CASCADE;
DROP TABLE IF EXISTS concern CASCADE;
DROP TABLE IF EXISTS _user CASCADE;
DROP TABLE IF EXISTS mail_parameter CASCADE;
DROP TABLE IF EXISTS bill_parameter CASCADE;
DROP TABLE IF EXISTS product CASCADE;
DROP TABLE IF EXISTS product_category CASCADE;
DROP TABLE IF EXISTS store CASCADE;
DROP TABLE IF EXISTS sellout CASCADE;
DROP TABLE IF EXISTS person CASCADE;
DROP TABLE IF EXISTS store_chain CASCADE;
DROP TABLE IF EXISTS role CASCADE;

CREATE TABLE role(
   id SERIAL,
   name VARCHAR(50)  NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(name)
);

CREATE TABLE store_chain(
   id SERIAL,
   name TEXT NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(name)
);

CREATE TABLE person(
   id SERIAL,
   mail VARCHAR(255)  NOT NULL,
   name VARCHAR(255)  NOT NULL,
   surname VARCHAR(255) ,
   id_store_chain INTEGER,
   PRIMARY KEY(id),
   UNIQUE(id_store_chain,mail),
   FOREIGN KEY(id_store_chain) REFERENCES store_chain(id)
);

CREATE TABLE sellout(
   id SERIAL,
   receipt_date DATE NOT NULL,
   livraison_date DATE NOT NULL,
   id_person INTEGER NOT NULL,
   PRIMARY KEY(id),
   FOREIGN KEY(id_person) REFERENCES person(id)
);

CREATE TABLE store(
   id SERIAL,
   town VARCHAR(255)  NOT NULL,
   adress VARCHAR(255)  NOT NULL,
   logo_path VARCHAR(255)  NOT NULL,
   id_store_chain INTEGER NOT NULL,
   id_person INTEGER NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(id_person),
   UNIQUE(logo_path),
   FOREIGN KEY(id_store_chain) REFERENCES store_chain(id),
   FOREIGN KEY(id_person) REFERENCES person(id)
);

CREATE TABLE product_category(
   id SERIAL,
   name VARCHAR(255)  NOT NULL,
   id_store INTEGER NOT NULL,
   id_container INTEGER,
   PRIMARY KEY(id),
   FOREIGN KEY(id_store) REFERENCES store(id),
   FOREIGN KEY(id_container) REFERENCES product_category(id)
);

CREATE TABLE product(
   id SERIAL,
   name VARCHAR(255)  NOT NULL,
   price INTEGER,
   id_product_category INTEGER NOT NULL,
   PRIMARY KEY(id),
   FOREIGN KEY(id_product_category) REFERENCES product_category(id)
);

CREATE TABLE bill_parameter(
   id SERIAL,
   name VARCHAR(255)  NOT NULL,
   foot VARCHAR(255)  NOT NULL DEFAULT 'placeholder',
   special_mention VARCHAR(255) ,
   mention VARCHAR(255) ,
   id_store INTEGER NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(name,id_store),
   UNIQUE(foot),
   FOREIGN KEY(id_store) REFERENCES store(id)
);



CREATE TABLE buy(
   id_store INTEGER,
   id_person INTEGER,
   PRIMARY KEY(id_store, id_person),
   FOREIGN KEY(id_store) REFERENCES store(id),
   FOREIGN KEY(id_person) REFERENCES person(id)
);


CREATE TABLE mail_parameter(
   id SERIAL,
   name VARCHAR(255)  NOT NULL,
   login VARCHAR(255)  NOT NULL,
   connexion_type VARCHAR(255)  NOT NULL,
   server VARCHAR(255) ,
   password VARCHAR(255)  NOT NULL,
   port INT NOT NULL,
   id_store INTEGER NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(name,id_store),
   UNIQUE(login),
   FOREIGN KEY(id_store) REFERENCES store(id)
);

CREATE TABLE _user(
   id SERIAL,
   pseudo VARCHAR(20)  NOT NULL,
   password VARCHAR(255)  NOT NULL,
   id_person INTEGER NOT NULL,
   id_store INTEGER,
   id_role INTEGER NOT NULL,
   PRIMARY KEY(id),
   UNIQUE(id_person),
   UNIQUE(pseudo),
   UNIQUE(password),
   FOREIGN KEY(id_person) REFERENCES person(id),
   FOREIGN KEY(id_store) REFERENCES store(id),
   FOREIGN KEY(id_role) REFERENCES role(id)
);

CREATE TABLE concern(
   id_product INTEGER,
   id_sellout INTEGER,
   quantity INTEGER NOT NULL ,
   PRIMARY KEY(id_product, id_sellout),
   FOREIGN KEY(id_product) REFERENCES product(id),
   FOREIGN KEY(id_sellout) REFERENCES sellout(id),
   CHECK(quantity>0)
);

CREATE TABLE contain(
   id_product INTEGER,
   id_menu INTEGER,
   quantity INTEGER NOT NULL,
   PRIMARY KEY(id_product, id_menu),
   FOREIGN KEY(id_product) REFERENCES product(id),
   FOREIGN KEY(id_menu) REFERENCES product(id),
   CHECK(quantity>0)
);


