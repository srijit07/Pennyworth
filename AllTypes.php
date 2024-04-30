<?php
    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');

    if (mysqli_connect_error()) 
    {
        echo "ERR1: Connection failed.";
        exit();
    }
    try
    {
        $username = $_POST["name"];
        $firstDateOfMonth = date('Y-m-01');
        $currentDate = date('Y-m-d');
        $TotalExpenseOfAllTypes = "SELECT type FROM `Expenses` WHERE username = ?";

        $stmt = $con->prepare($TotalExpenseOfAllTypes);
        if ($stmt === false)
            throw new Exception("Failed to prepare statement: " . $con->error);

        $stmt->bind_param("s", $username);
        $stmt->execute();
        $result = $stmt->get_result();

        if ($result->num_rows > 0) 
        {
            while ($row = $result->fetch_assoc())
            {
                echo $row['type'] . "*@%>"
                ;
            }
        } 
        else
            exit();
        $stmt->close();
    }

    catch (Exception $e) 
    {
        echo "An error occurred: " . $e->getMessage();
    }
    $con -> close();