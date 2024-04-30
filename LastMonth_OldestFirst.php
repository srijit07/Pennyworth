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
        $currentDate = date('Y-m-d');
        $firstDateOfMonth = date('Y-m-01');

        $LastMonth = "SELECT * FROM `Expenses` WHERE username = ? AND date BETWEEN ? AND ? order by date, time;";
        $stmt = $con -> prepare($LastMonth);

        $stmt->bind_param("sss", $username, $firstDateOfMonth, $currentDate);

        $exec = $stmt->execute();

        $results = $stmt->get_result()->fetch_all(MYSQLI_ASSOC);

        if(empty($results))
        {
            echo "Result set is empty!";
        }
        else
        {
            foreach($results as $row)   
            {
                echo htmlspecialchars($row["id"]) . "*@%>";
                echo htmlspecialchars($row["username"]) . "*@%>";
                echo htmlspecialchars($row["type"]) . "*@%>";
                echo htmlspecialchars($row["message"]) . "*@%>";
                echo htmlspecialchars($row["amount"]) . "*@%>";
                echo htmlspecialchars($row["date"]) . "*@%>";
                echo htmlspecialchars($row["time"]);
                echo "&%<$";
                echo "<br>"; 
            }
        }
        $stmt->close();
        
    }

    catch(Exception $e)
    {
        echo $e->getMessage();
    }
    $con->close();
