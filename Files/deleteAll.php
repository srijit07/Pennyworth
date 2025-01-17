<?php
    $con = mysqli_connect('localhost', 'id22029474_dbsminiproject', 'Freesqldb@24', 'id22029474_dbsminiproject');
    if (mysqli_connect_error()) 
    {
        echo "ERR1: Connection failed.";
        exit();
    }

    try {
        $id = $_POST["id"];
        $query = "DELETE FROM Expenses WHERE id = ?";
        $stmt = $con->prepare($query);
        $stmt->bind_param("s", $id);
        $stmt->execute();

        if ($stmt->affected_rows > 0)
            echo "Expense deleted successfully.";
        else 
        {
            echo "No expense found with the provided ID.";
        }

        $stmt->close();
    } catch (Exception $e) {
        echo $e->getMessage();
        exit();
    }

    $con->close();

