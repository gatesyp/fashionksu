<?php
$servername = "kinect.cijwsnhnawhn.us-east-1.rds.amazonaws.com";
$username = "root";
$password = "lollipop";
$dbname = "kinect";
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
