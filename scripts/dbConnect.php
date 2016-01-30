<?php
$servername = "stoh.io";
$username = "fashionksu";
$password = "lollipop123";
$dbname = "fashionksu";
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
} 
echo "Connected successfully";
