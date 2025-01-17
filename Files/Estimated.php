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
        $firstDateOfMonth = date('Y-m-01');

        $currentDate = date('Y-m-d');
        $currentDateTime = new DateTime($currentDate);
        $threeMonthsAgo = $currentDateTime->modify('-3 months')->format('Y-m-01');
        $endDate = $currentDateTime->modify('last day of last month')->format('Y-m-d');

        $seventhDay = date('Y-m-07');

        $dateFetchQuery = "SELECT date FROM Users WHERE username = ?";
        $stmt = $con->prepare($dateFetchQuery);
        $stmt->bind_param("s", $username);
        $stmt->execute();
        $result = $stmt->get_result();
        if ($row = $result->fetch_assoc())
            $fetchedDate = $row['date'];
        else
        {
            //echo "No date found for the given username.";
            exit();
        }

        $query = "SELECT DATEDIFF(?, ?) AS difference";
        $stmt2 = $con->prepare($query);
        $stmt2->bind_param("ss", $currentDate, $fetchedDate);
        $stmt2->execute();
        $result2 = $stmt2->get_result();
        if ($row2 = $result2->fetch_assoc())
        {
            $difference = intval($row2['difference']);
        }
        else 
        {
            //echo "Error calculating date difference.";
            exit();
        }

        if ($difference >= 7 && $difference < 90) {
            $query1 = "SELECT ROUND((SUM(amount)/DATEDIFF(?, ?)) * 30, 2) AS estimated_expense FROM Expenses WHERE username = ? AND date BETWEEN ? AND ?";
            $stmt3 = $con->prepare($query1);
            $stmt3->bind_param("sssss", $seventhDay, $firstDateOfMonth, $username, $firstDateOfMonth, $seventhDay);
        } 
        elseif ($difference >= 90) 
        {
            $query2 = "SELECT ROUND((SUM(amount)/DATEDIFF(?, ?)) * 30, 2) AS estimated_expense FROM Expenses WHERE username = ? AND date BETWEEN ? AND ?";
            $stmt3 = $con->prepare($query2);
            $stmt3->bind_param("sssss", $endDate, $threeMonthsAgo, $username, $threeMonthsAgo, $endDate);
        } 
        else
        {
            //echo "Difference in days is less than 7, no estimation needed.";
            exit();
        }

        $stmt3->execute();
        $result3 = $stmt3->get_result();
        if ($row3 = $result3->fetch_assoc())
            echo $row3["estimated_expense"];
        else 
        {
            echo "Error calculating estimated expense.";
            exit();
        }

        $stmt->close();
        $stmt2->close();
        $stmt3->close();
    } 
    catch (Exception $e) 
    {
        echo $e->getMessage();
    }

    $con->close();

