DELIMITER //

CREATE OR REPLACE TRIGGER update_bill_parameter_name
BEFORE INSERT ON bill_parameter
FOR EACH ROW
BEGIN
    IF NEW.name IS NULL THEN
        SET NEW.name = (SELECT name FROM store WHERE id = NEW.id_store);
    END IF;
END //

CREATE OR REPLACE TRIGGER update_mail_parameter_name
BEFORE INSERT ON mail_parameter
FOR EACH ROW
BEGIN
    IF NEW.name IS NULL THEN
        SET NEW.name = (SELECT name FROM store WHERE id = NEW.id_store);
    END IF;
END //

CREATE OR REPLACE TRIGGER check_client_before_insert
BEFORE INSERT ON buy
FOR EACH ROW
BEGIN
    DECLARE store_chain INT;
    DECLARE person_store_chain INT;

    SELECT id_store_chain INTO store_chain
    FROM store
    WHERE id = NEW.id_store;

    SELECT id_store_chain INTO person_store_chain
    FROM person
    WHERE id = NEW.id_person;

    IF store_chain <> person_store_chain THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'New client error';
    END IF;
END //

CREATE OR REPLACE TRIGGER update_client_ref
BEFORE CREATE OR UPDATE ON buy
FOR EACH ROW
BEGIN
    UPDATE person SET last_used=NOW() WHERE id=NEW.id_person
    RETURN NEW
END//


CREATE OR REPLACE TRIGGER update_user_ref
BEFORE CREATE OR UPDATE ON user
FOR EACH ROW
BEGIN
    UPDATE person SET last_used=NOW() WHERE id=NEW.id_person
    RETURN NEW
END//

CREATE OR REPLACE TRIGGER check_client_before_update
BEFORE UPDATE ON buy
FOR EACH ROW
BEGIN
    DECLARE store_chain INT;
    DECLARE person_store_chain INT;

    SELECT id_store_chain INTO store_chain
    FROM store
    WHERE id = NEW.id_store;

    SELECT id_store_chain INTO person_store_chain
    FROM person
    WHERE id = NEW.id_person;

    IF store_chain <> person_store_chain THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'New client error';
    END IF;
END //


CREATE OR REPLACE TRIGGER default_bill_parameter
BEFORE INSERT ON bill_parameter
FOR EACH ROW 
BEGIN
    IF NEW.mention IS NULL THEN
        NEW.mention:='AVEC LES REMERCIEMENT DE '||(SELECT name FROM full_store WHERE id=NEW.id_store);
    END IF;
    RETURN NEW;
END //

DELIMITER ;
