<?php

$config = require 'config.php';
$dbConfig = $config['database'];

$connection = mysqli_connect(
    $dbConfig['host'],
    $dbConfig['username'],
    $dbConfig['password'],
    $dbConfig['dbname']
);

if (!$connection) {
    die("Connection failed: " . mysqli_connect_error() . "\n");
}

$sql = file_get_contents('database_setup.sql');

if ($sql === false) {
    die("Error reading database_setup.sql file\n");
}

// Split SQL by semicolon to handle multiple statements
$queries = array_filter(
    array_map(
        'trim',
        explode(';', $sql)
    ),
    'strlen'
);

$success = true;
foreach ($queries as $query) {
    if (!mysqli_query($connection, $query)) {
        echo "Error executing query: " . mysqli_error($connection) . "\n";
        echo "Query: " . $query . "\n";
        $success = false;
        break;
    }
}

if ($success) {
    echo "Database setup completed successfully\n";
}

mysqli_close($connection);
