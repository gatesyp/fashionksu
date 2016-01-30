<?php
require 'dbConnect.php';
$sql = "SELECT first.profile AS similarPerson, second.response AS sharedResponse, second.url AS sharedURL
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON second.profile !=  '1022691251124985'
AND first.profile !=  '1022691251124985'
AND second.url = first.url
AND second.response = first.response";

// var_dump($sql);
// $sql = "SELECT first.* FROM profile_data AS first WHERE first.profile = \'1022691251124985\' and first.response = (select response from profile_data AS second where second.profile != first.profile and second.url = first.url and second.response = first.response)";
  if($result = $conn -> query($sql)){
    var_dump($result -> fetch_object());
    while($row = $result -> fetch_object()){
      $id = $row -> id;
    }
  }
