CREATE OR REPLACE FUNCTION rename_bill() RETURNS TRIGGER AS $$
BEGIN
IF NEW.name IS NULL THEN
    SELECT name INTO NEW.name
        FROM store
        WHERE id = NEW.id_store;
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_bill_parameter_name
BEFORE INSERT OR UPDATE ON bill_parameter
FOR EACH ROW
EXECUTE FUNCTION rename_bill();

CREATE OR REPLACE FUNCTION rename_mail() RETURNS TRIGGER AS $$
BEGIN
IF NEW.name IS NULL THEN
    SELECT name INTO NEW.name
        FROM store
        WHERE id = NEW.id_store;
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_mail_parameter_name
BEFORE INSERT OR UPDATE ON mail_parameter
FOR EACH ROW
EXECUTE FUNCTION rename_mail();

CREATE OR REPLACE FUNCTION check_compatibility() RETURNS TRIGGER AS $$
BEGIN
IF (SELECT id_store_chain FROM store WHERE id=NEW.id_store)!=(SELECT id_STORE_chain FROM person WHERE id=NEW.id_person) THEN
    RETURN NEW;
ELSE 
    RAISE EXCEPTION 'New client error' USING HINT = 'PERSON AND STORE MUST BE KNOW BY THE SAME store_chain';
END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER check_client
BEFORE INSERT OR UPDATE ON buy
FOR EACH ROW
EXECUTE FUNCTION check_compatibility();