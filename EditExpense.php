<?php
$con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');
if (mysqli_connect_error()) 
{
    echo "ERR1: Connection failed.";
    exit();
}

try {
    $id = $_POST["id"];
    $type = $_POST["type"];
    $message = $_POST["message"];
    $amount = $_POST["amount"];
    $date = $_POST["date"];
    $time = $_POST["time"];

    $updateQuery = "UPDATE Expenses SET type = ?, message = ?, amount = ?, date = ?, time = ? WHERE id = ?";
    $stmt = $con->prepare($updateQuery);
    $stmt->bind_param("ssdssi", $type, $message, $amount, $date, $time, $id);
    $stmt->execute();
    echo "0";
    $stmt->close();
} 
catch(Exception $e) 
{
    echo $e->getMessage();
}

$con->close();
?>
