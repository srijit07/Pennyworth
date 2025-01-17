<?php

    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');
    
    if (mysqli_connect_error())
    {
        echo "ERR1: Connection failed.";
        exit();
    }
    
    $username = $_POST["name"];
    $password = $_POST["password"];
    $date = $_POST["date"];
    
    $namecheckquery = "SELECT username from Users WHERE BINARY username = '" . $username . "';";
    $namecheck = mysqli_query($con, $namecheckquery) or die("ERR2: Name check failed.");
    
    if (mysqli_num_rows($namecheck) > 0)
    {
        echo "ERR3: Name already exists.";
        exit();
    }

    $hashedPassword = hash('sha256', $password);
    
    $insertuserquery = "INSERT INTO Users (username, password, date) VALUES ('" . $username . "', '" . $hashedPassword . "', '" . $date . "');";
    mysqli_query($con, $insertuserquery) or die("ERR4: Registration failed.");
    
    echo "0";

