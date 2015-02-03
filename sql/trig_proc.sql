CREATE OR REPLACE PROCEDURE proc_add_customer(
    p_credit_card_number varchar,
    p_credit_card_type smallint,
    p_credit_card_expiration_month smallint,
    p_credit_card_expiration_year smallint,
    p_credit_card_cvv varchar,
    p_id_package integer)
    IS
    BEGIN
        INSERT INTO customers 
            (id,
            credit_card_number,
            credit_card_type,
            credit_card_expiration_month,
            credit_card_expiration_year,
            credit_card_cvv,
            id_package)
        VALUES
            (seq_customers.NEXTVAL,
            p_credit_card_number,
            p_credit_card_type,
            p_credit_card_expiration_month,
            p_credit_card_expiration_year,
            p_credit_card_cvv,
            p_id_package);
    END proc_add_customer;

CREATE OR REPLACE TRIGGER trig_check_max_renting
BEFORE INSERT ON rentings
FOR EACH ROW
DECLARE
    current_package_max_rentings number;
    current_rentings_number number;
BEGIN
     SELECT packages.max_films INTO current_rentings_number
     FROM packages JOIN customers ON customers.id_package = packages.id
     WHERE customers.id = :NEW.id_customer;
        
     SELECT COUNT(*) INTO current_rentings_number
     FROM rentings JOIN customers ON customers.id = rentings.id_customer
     WHERE customers.id = :NEW.id_customer and rentings.return_date IS NULL;
    
     IF (current_rentings_number > current_package_max_rentings) THEN
     	RAISE_APPLICATION_ERROR(-20001, 'Cannot insert renting because the customer has exceeded the number of rented films of his package');
     END IF;
END trig_check_max_renting;

CREATE OR REPLACE PROCEDURE proc_add_renting(
    --p_id integer,
    p_id_customer integer,
    p_id_film integer--,
    --p_rent_date date
	) AS
    BEGIN
    INSERT INTO rentings
        (id,
        id_customer,
        id_film,
        rent_date)
    VALUES
        (seq_rentings.nextval,
        p_id_customer,
        p_id_film,
        SYSDATE);
    END proc_add_renting;
	
CREATE OR REPLACE TRIGGER trig_validate_customer_columns
BEFORE INSERT ON customers
FOR EACH ROW
DECLARE
v_birthdate date;
BEGIN
    IF (TO_DATE(:NEW.credit_card_expiration_year || '-' || :NEW.credit_card_expiration_month, 'YYYY-MM') < TO_DATE(TO_CHAR(SYSDATE, 'YYYY-MM'), 'YYYY-MM')) THEN
    	RAISE_APPLICATION_ERROR(-20001, 'Cannot insert customer because his credit card is expired.');
    END IF;
    
    SELECT birthdate INTO v_birthdate from persons WHERE id = :NEW.id;
    
    IF (MONTHS_BETWEEN(SYSDATE, v_birthdate)/12 < 18) THEN
    	RAISE_APPLICATION_ERROR(-20001, 'Cannot insert customer because the related person is not 18 years old.');
    END IF;
END trig_validate_customer_columns;

--CREATE OR REPLACE TRIGGER trig_update_inv_on_rent
--AFTER INSERT ON rentings
--FOR EACH ROW
--BEGIN
--	UPDATE films SET number_of_copies = number_of_copies - 1
--	WHERE id = :NEW.id_film;
--END trig_update_inv_on_rent;
--
--CREATE OR REPLACE TRIGGER trig_update_inv_on_ret
--AFTER UPDATE OF return_date ON rentings
--REFERENCING
--	NEW AS new_line
--FOR EACH ROW
--WHEN (new_line.return_date IS NOT NULL)
--BEGIN
--	UPDATE films SET number_of_copies = number_of_copies + 1
--	WHERE id = new_line.id_film;
--END trig_update_inv_on_ret;

CREATE OR REPLACE PROCEDURE proc_return_renting(
    p_id_customer integer,
    p_id_film integer,
    p_return_date date)
    IS
    BEGIN
		UPDATE films SET return_date = p_return_date
		WHERE id_customer = p_id_customer AND id_film = p_id_film;
    END proc_return_renting;
