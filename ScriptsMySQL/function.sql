DELIMITER $$
CREATE FUNCTION is_menu(id INT) RETURNS BOOLEAN
BEGIN
    DECLARE menu_exists BOOLEAN;
    SELECT EXISTS(SELECT * FROM contain WHERE id_menu = id) INTO menu_exists;
    RETURN menu_exists;
END$$
DELIMITER ;

DELIMITER $$
CREATE FUNCTION is_client(id INT) RETURNS BOOLEAN
BEGIN
    DECLARE client_not_exists BOOLEAN;
    SELECT NOT EXISTS(
        SELECT * FROM store
        LEFT JOIN _user ON _user.id_person = id
        WHERE store.id_person = id
    ) INTO client_not_exists;
    RETURN client_not_exists;
END$$
DELIMITER ;

DELIMITER $$
CREATE FUNCTION check_cat_compability(id_reference INT, id_comparison INT) RETURNS BOOLEAN
BEGIN
    DECLARE cat_compatible BOOLEAN;
    SELECT (SELECT id_store FROM product_category JOIN product ON id_product_category = product_category.id WHERE productid = id_reference)
    = (SELECT id_store FROM product_category JOIN product ON id_product_category = product_category.id WHERE productid = id_comparison)
    INTO cat_compatible;
    RETURN cat_compatible;
END$$
DELIMITER ;