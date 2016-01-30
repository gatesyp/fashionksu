<?php
require 'dbConnect.php';
$sql = "SELECT first.* FROM profile_data AS first WHERE first.profile = \'1022691251124985\' and first.response = (select response from profile_data AS second where second.profile != first.profile and second.url = first.url and second.response = first.response)";
  if($result = $conn -> query($sql)){
    var_dump($result);
    while($row = $result -> fetch_object()){
      $id = $row -> id;
    }
  }
