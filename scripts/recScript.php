<?php
require 'dbConnect.php';

$agreements = [];
$conflicts = [];
$profiles = [];

$sql = 'SELECT DISTINCT profile from profile_data';
$result = $conn->query($sql);
while ($row = $result->fetch_assoc()) {
    $profiles[$row['profile']] = [];
}

// grab: l1 ^ L2 + D1 ^ D2
// magnitude of ratings which users are similar on
$mainProfile = "1075680275800091";
$sql = "SELECT FIRST.profile AS similarPerson, COUNT( * ) as likes
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON FIRST.profile !=  '" . $mainProfile . "'
AND SECOND.profile =  '" . $mainProfile . "'
AND SECOND.url = FIRST.url
AND SECOND.response = FIRST.response
GROUP BY similarPerson
ORDER BY similarPerson DESC";
$result = $conn->query($sql);

// now process the result set and count the occurrences of each person, as compared to our original person P
if (!$result = $conn->query($sql)) {
    return;
}

while ($row = $result->fetch_assoc()) {
    $profiles[$row['similarPerson']]['likes'] = $row['likes'];
}

// grab: L1 ^ D2 + L2 ^ D1
// magnitude of ratings which users disagree on
$sql = "SELECT COUNT( * ) as conflict, FIRST.profile AS similarPerson
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON FIRST.profile !=  '" . $mainProfile . "'
AND SECOND.profile =  '" . $mainProfile . "'
AND SECOND.url = FIRST.url
AND SECOND.response != FIRST.response
GROUP BY similarPerson
ORDER BY similarPerson DESC";

// now process the result set and count occurrences of each person, as compared to our original person P
if (!$result = $conn->query($sql)) {
    return;
}

while ($row = $result->fetch_assoc()) {
    $profiles[$row['similarPerson']]['conflict'] = $row['conflict'];
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// denominator
// grab: l1 U L2 U D1 U D2

$sql = "SELECT COUNT( * ) AS frequency, profile AS name
FROM profile_data
GROUP BY profile
ORDER BY profile DESC ";
$i = 0;
$denom = [];
if ($result = $conn->query($sql)) {
    while ($row = $result->fetch_object()) {
        $temp = ['profile' => $row->profile, 'frequency' => $row->frequency];
        $denom[ $i ] = $temp;
        $i += 1;
    }
}

// add the two magnitudes to get the final numerator, and divide by denominator to get similarity indices
var_dump($profiles);
exit;
$vals = [];
$i = 0;
foreach ($agreements as $key => $value) {
    $vals[ $i ] = (floatval($value["frequency"]) - floatval($conflicts[ $i ]["frequency"])) / floatval($denom[ $i ]["frequency"]);
    $i += 1;
}
// var_dump($vals);

//insert the calculated similarity indices
$i = 0;
foreach ($vals as $key => $rating) {
    // search to see if key exists
    $sql = "SELECT id FROM similarity_index WHERE main_profile = '" . $mainProfile . "' and profile = '" . $conflicts[ $i ]["profile"] . "'";
    // var_dump($sql);
    // if exists perform update
    if ($result = $conn->query($sql)) {
        while ($row = $result->fetch_object()) {
            $id = $row->id;
            echo "going to do an update";
        }
    } else {
        $sql = "INSERT INTO similarity_index(main_profile, profile, rating) VALUES('" . $mainProfile . "', '" . $conflicts[ $i ]["profile"] . "', " . $rating . ")";
        var_dump($sql);
        if ($result = $conn->query($sql)) {
            echo "inserted good";
        }
    }
    $i += 1;
}
