<?php

    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');
    
    if (mysqli_connect_error())
    {
        echo "ERR1: Connection failed.";
        exit();
    }
    
    $username = $_POST["name"];
    $password = $_POST["password"];
    
    $hashedPassword = hash('sha256', $password);
    
    $namecheckquery = "SELECT username, password from Users WHERE BINARY username = '" . $username . "' AND BINARY password = '" . $hashedPassword . "';";
    $namecheck = mysqli_query($con, $namecheckquery) or die("ERR2: Name check failed.");
    
    
    if (mysqli_num_rows($namecheck) == 0)
    {
        echo "ERR5: No such account found.";
        exit();
    }
    
    echo "0";

?>