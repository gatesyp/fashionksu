<?php
// ALTER TABLE `profile_data` CHANGE `url` `url_id` INT(63) UNSIGNED NOT NULL;
session_start();

require 'dbConnect.php';
require 'vendor/autoload.php';

use DirkGroenen\Pinterest\Pinterest;

if (1 == 2) {
    $pinterest = new Pinterest("4815503224578518879", "2b3b2d7158fd5d0cad85784bec3400a2e238049c89e760185cf9191be4d846ea");
    $url = 'https://' . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI'];

    if (isset($_GET["code"]) && empty($_SESSION['token'])) {
        $token = $pinterest->auth->getOAuthToken($_GET["code"]);
        $_SESSION['token'] = $token['token'];
    }

    if (!empty($_SESSION['token'])) {
        $pinterest->auth->setOAuthToken($_SESSION['token']);
    }

    if (empty($_SESSION['token'])) {
        $loginurl = $pinterest->auth->getLoginUrl($url, ['read_public']);
        echo '<a href=' . $loginurl . '>Authorize Pinterest</a>';

        return;
    }
    $mainProfile = $pinterest->users->me()->id;
}

$mainProfile = '1075680275800091';

if(isset($_GET['update']) && !empty($_GET['id']) && !empty($_GET['response'])) {
    $sql = 'INSERT INTO profile_data(profile, url_id, response) VALUES ("'.$mainProfile.'", "'.$_GET['id'].'", "'.$_GET['response'].'")';
    $result = $conn->query($sql);

    return;
}

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

    if (!$result = $conn->query($sql)) {
        return;
    }

    while ($row = $result->fetch_assoc()) {
        $profiles[ $row['similarPerson'] ]['likes'] = $row['likes'];
    }

    $sql = "SELECT COUNT( * ) as conflict, FIRST.profile AS similarPerson
FROM profile_data
FIRST INNER JOIN profile_data
SECOND ON FIRST.profile !=  '" . $mainProfile . "'
AND SECOND.profile =  '" . $mainProfile . "'
AND SECOND.url = FIRST.url
AND SECOND.response != FIRST.response
GROUP BY similarPerson
ORDER BY similarPerson DESC";

    if (!$result = $conn->query($sql)) {
        return;
    }

    while ($row = $result->fetch_assoc()) {
        $profiles[ $row['similarPerson'] ]['conflict'] = $row['conflict'];
    }

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

//////////////////////////////////////////////////////////////////////////////////////////////////
if (isset($_GET['get_suggests'])) {
    $sql = 'SELECT first.url_id, pics.url as url
FROM profile_data as first,
    pictures as pics
WHERE profile = (
    SELECT profile FROM `similarity_index`
    WHERE main_profile = "' . $mainProfile . '"
    ORDER BY rating DESC
    LIMIT 1
)
and response = 1
and url_id not IN (
	SELECT url_id FROM profile_data WHERE profile = "' . $mainProfile . '"
)
and url_id = pics.id
';

    $result = $conn->query($sql);
    $urls = [];
    while ($row = $result->fetch_assoc()) {
        $urls[ $row['url_id'] ] = $row['url'];
    }

    $sql = 'SELECT url, id from pictures LIMIT 20';
    $result = $conn->query($sql);
    while ($row = $result->fetch_assoc()) {
        $urls[ $row['id'] ] = $row['url'];
    }

    echo json_encode(['suggestions' => $urls]);
}
//////////////////////////////////////////////////////////////////////////////////////////////////
