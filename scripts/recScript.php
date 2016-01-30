<?php
require 'dbConnect.php';
// grab: l1 ^ L2 + D1 ^ D2
// magnitude of ratings which users are similar on
$sql = "SELECT FIRST.profile AS similarPerson, SECOND.response AS sharedResponse, SECOND.url AS sharedURL
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON SECOND.profile !=  '1022691251124985'
AND FIRST.profile !=  '1022691251124985'
AND SECOND.url = FIRST.url
AND SECOND.response = FIRST.response";

// now process the result set and count the occurrences of each person, as compared to our original person P




// grab: L1 ^ D2 + L2 ^ D1
// magnitude of ratings which users disagree on
$sql = "SELECT FIRST.profile AS similarPerson, SECOND.response AS sharedResponse, SECOND.url AS sharedURL
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON SECOND.profile !=  '1022691251124985'
AND FIRST.profile !=  '1022691251124985'
AND SECOND.url = FIRST.url
AND SECOND.response != FIRST.response";

// now process the result set and count occurrences of each person, as compared to our original person P

// flip sign on the magnitudes

// add the two magnitudes to get the final numerator


// denominator:
// grab: l1 U L2 U D1 U D2
$sql = "SELECT * WHERE profile = 'zxczxczxc' and profile = 'vbnvbnvbn'";
$total_sum = 0;
if($result = $conn -> query($sql)){
  while($row = $result -> fetch_object()){
    // count how many rows are returned for each profile
    $total_sum += 1;
  }
}
