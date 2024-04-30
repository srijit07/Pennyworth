<?php
    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');

    if (mysqli_connect_error())
    {
        echo "ERR1: Connection failed.";
        exit();
    }

    // Prepare an insert statement
    $query = "INSERT INTO `Expenses` (username, type, message, amount, date, time) VALUES (?, ?, ?, ?, ?, ?)";
    $stmt = $con->prepare($query);

    // Check if the prepare was successful
    if (false === $stmt) {
        // Handle error here
        die("ERR2: Prepare failed: " . htmlspecialchars($con->error));
    }

    //$id = 1234;
    $username = $_POST["name"];
    $type = $_POST["type"];
    $message = $_POST["message"];
    $amount = $_POST["amount"];
    $date = $_POST["date"];
    $time = $_POST["time"];

    // 'i' corresponds to integer, 's' to string, and 'd' to double (float)
    $bind = $stmt->bind_param("sssdss", $username, $type, $message, $amount, $date, $time);

    if (false === $bind)
        die("ERR3: Bind failed: " . htmlspecialchars($stmt->error));

    $exec = $stmt->execute();

    if (false === $exec)
        die("ERR4: Execute failed: " . htmlspecialchars($stmt->error));
    else
        echo "0";

    $stmt->close();

    $con->close();

