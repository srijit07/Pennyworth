<?php
    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');
    if (mysqli_connect_error())
    {
        echo "ERR1: Connection failed.";
        exit();
    }

    try
    {

        $displayAll = "SELECT * FROM `Expenses`;";
        $stmt = $con -> prepare($displayAll);

        $exec = $stmt->execute();

        $results = $stmt->get_result()->fetch_all(MYSQLI_ASSOC);

        if(empty($results))
        {
            echo "Result set is empty!";
        }
        else
        {
            var_dump($results);
        }
        $stmt->close();
        
    }

    catch(Exception $e)
    {
        echo $e->getMessage();
    }
    $con->close();
