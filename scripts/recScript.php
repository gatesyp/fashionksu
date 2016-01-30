<?php
// ALTER TABLE `similarity_index` ADD UNIQUE `profile_index`(`main_profile`, `profile`);

require 'dbConnect.php';

if (empty($_GET['profile'])) {
    echo 'Missing profile data';

    return;
}

$mainProfile = $_GET['profile'];

//////////////////////////////////////////////////////////////////////////////////////////////////

if (isset($_GET['recalculate'])) {

    $agreements = [];
    $conflicts = [];
    $profiles = [];

    $sql = 'SELECT DISTINCT profile from profile_data';
    $result = $conn->query($sql);
    while ($row = $result->fetch_assoc()) {
        $profiles[ $row['profile'] ] = [];
    }

// grab: l1 ^ L2 + D1 ^ D2
// magnitude of ratings which users are similar on
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
        $profiles[ $row['similarPerson'] ]['likes'] = $row['likes'];
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
        $profiles[ $row['similarPerson'] ]['conflict'] = $row['conflict'];
    }

// denominator
// grab: l1 U L2 U D1 U D2

    $denom = [];
    $sql = "SELECT COUNT( * ) AS frequency, profile AS name
FROM profile_data
GROUP BY profile
ORDER BY profile DESC ";
    $result = $conn->query($sql);
    while ($row = $result->fetch_assoc()) {
        $profiles[ $row['name'] ]['totalSwiped'] = $row['frequency'];
    }

    foreach ($profiles as $theguy => $profile) {
        $profiles[ $theguy ]['ratio'] = (floatval($profile['likes']) - floatval($profile["conflict"])) / floatval($profile['totalSwiped']);
    }

    foreach ($profiles as $key => $profile) {
        $sql = sprintf(
            "INSERT INTO similarity_index (main_profile, profile, rating)
            VALUES ('%s', '%s','%s')
            ON DUPLICATE KEY UPDATE
            rating='%s'",
            $mainProfile, $key, $profile['ratio'], $profile['ratio']
        );
        $result = $conn->query($sql);
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////

if (isset($_GET['get_suggests'])) {
    $sql = 'SELECT url FROM profile_data as first
WHERE profile = (
    SELECT profile FROM `similarity_index`
    WHERE main_profile = "' . $mainProfile . '"
    ORDER BY rating DESC
    LIMIT 1
)
and response = 1
and URL not IN (
	SELECT url FROM profile_data WHERE profile = "' . $mainProfile . '"
)';

    $result = $conn->query($sql);
    $urls = [];
    while ($row = $result->fetch_assoc()) {
        $urls[] = $row['url'];
    }

    var_dump($urls);
    exit;
}

//////////////////////////////////////////////////////////////////////////////////////////////////
