<?php
$servername = "localhost";
$username = "id21959871_dbmsminiproject";
$password = "Krisha@11J";
$database = "id21959871_dbmsminiproject";

// Attempt to connect to MySQL database
try
{
    $conn = mysqli_connect($servername, $username, $password, $database);
}

catch(Exception $e)

{
    echo "". $e->getMessage();
}

