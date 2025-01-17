<?php
    echo "0.5.1";

DECLARE
    v_name varchar(10);
    CURSOR cur_name (var_name NUMBER := 100) IS 
    SELECT name from students where id > 10;

BEGIN
    LOOP
        OPEN cur_name(150);
        FETCH cur_name INTO v_name;
        EXIT WHEN cur_name%NOTFOUND;
    END LOOP;
    CLOSE cur_name;
END;

/

DECLARE
    v_string varchar(50);
    v_reverseString varchar(50);
    v_length NUMBER;
BEGIN
    v_string:= :Input_String;
    v_length := LENGTH(v_string);
    FOR i IN 1..v_length LOOP
        v_reverseString := SUBSTR(v_string, i, 1) || v_reverseString;
    END LOOP;
    DBMS_OUTPUT.PUT_LINE('Reversed String: '|| v_reverseString);
END;

/

DECLARE
    v_charles_leclerc INTEGER;
    v_fact INTEGER := 1;
BEGIN
    v_charles_leclerc := :v_prompt;

    FOR i IN 1..v_charles_leclerc LOOP
        v_fact := v_fact * i;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('Factorial of '||v_charles_leclerc||' is '||v_fact);
END;