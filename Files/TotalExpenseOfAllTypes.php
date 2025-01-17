<?php
    date_default_timezone_set('Asia/Kolkata');
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
        $TotalExpenseOfAllTypes = "SELECT type, SUM(amount) AS TotalAmount FROM `Expenses` WHERE username = ? AND date BETWEEN ? AND ? GROUP BY Type";

        $stmt = $con->prepare($TotalExpenseOfAllTypes);
        if ($stmt === false)
            throw new Exception("Failed to prepare statement: " . $con->error);

        $stmt->bind_param("sss", $username, $firstDateOfMonth, $currentDate);
        $stmt->execute();
        $result = $stmt->get_result();

        if ($result->num_rows > 0) 
        {
            while ($row = $result->fetch_assoc())
            {
                echo $row['type'] . "*@%>". $row['TotalAmount'] . "&%<$";
                echo "<br>";
            }
        } 
        else
            echo "No records found.";
        $stmt->close();
    }

    catch (Exception $e) 
    {
        echo "An error occurred: " . $e->getMessage();
    }
    $con -> close();