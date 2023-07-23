CREATE OR REPLACE FUNCTION is_menu(id INT) RETURNS BOOLEAN AS $$
BEGIN
    SELECT * FROM contain WHERE id_menu=id;
    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION is_client(id int) RETURNS BOOLEAN AS $$
BEGIN
    SELECT * FROM store  
        LEFT JOIN _user ON _user.id_person=id
        WHERE store.id_person=id;
    RETURN !FOUND;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION check_cat_compability(id_reference int,id_comparison int) RETURNS BOOLEAN AS $$
BEGIN
    RETURN (SELECT id_store FROM product_category JOIN product ON id_product_category=product_category.id WHERE productid=id_reference)=(SELECT id_store FROM product_category JOIN product ON id_product_category=product_category.id WHERE productid=id_comparaison);
END;
$$ LANGUAGE plpgsql;