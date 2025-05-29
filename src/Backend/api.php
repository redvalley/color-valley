<?php
require_once "highscore_service.php";


header("Content-Type: application/json");

// Get the HTTP method, path, and input
$method = $_SERVER['REQUEST_METHOD'];
$request = explode('/', trim($_SERVER['PATH_INFO'] ?? '', '/'));

$payload = file_get_contents('php://input');



// Example: /users/1
$resource = implode('/', $request);
$id = $request[1] ?? null;


$highScoreService = new HighScoreService();


// Routing logic
switch ($method) {
    case 'GET':
        if ($resource === 'scores/top-overall') {
            $highScores = $highScoreService->getOverallTopScores();
            echo json_encode($highScores);
            break;
        }

        if ($resource === 'scores/top-current-month') {
            $highScores = $highScoreService->getCurrentMonthTopScores();
            echo json_encode($highScores);
            break;
        }

        if ($resource === 'scores/top-today') {
            $highScores = $highScoreService->getTodayTopScores();
            echo json_encode($highScores);
            break;
        }

        break;

    case 'POST':
        if ($resource === 'scores') {
            $highScoreEntry = HighScoreEntry::fromJson($payload);
            echo json_encode($highScoreService->addHighScore($highScoreEntry));
        }
        break;

    case 'PUT':
        break;

    case 'DELETE':
        break;

    default:
        http_response_code(405);
        echo json_encode(['error' => 'Method not allowed']);
        break;
}
