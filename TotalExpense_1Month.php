<?php
    date_default_timezone_set('Asia/Kolkata');
    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');

    if (mysqli_connect_error()) 
    {
        echo "ERR1: Connection failed.";
        exit();
    }

    try {
        $username = $_POST["name"];

        $currentDate = date('Y-m-d');
        $firstDateOfMonth = date('Y-m-01');

        $TotalExpense_CurrentToFirstOfMonth = "SELECT SUM(amount) AS total_amount FROM `Expenses` WHERE username = ? AND date BETWEEN ? AND ?";
        $stmt = $con->prepare($TotalExpense_CurrentToFirstOfMonth);

        $stmt->bind_param("sss", $username, $firstDateOfMonth, $currentDate);

        $exec = $stmt->execute();
        $result = $stmt->get_result();
        $row = $result->fetch_assoc();

        if ($row)
            echo $row["total_amount"];
        else
            echo "Result set is empty!";
        $stmt->close();
        } 
        catch (Exception $e) 
        {
            echo $e->getMessage();
        }

        $con->close();
