<?php

require_once "high_score_entry.php";
require_once "config.php";

class HighScoreService {
    private mysqli $connection;

    public function __construct() {
        $config = require 'config.php';
        $dbConfig = $config['database'];

        $this->connection = mysqli_connect(
            $dbConfig['host'],
            $dbConfig['username'],
            $dbConfig['password'],
            $dbConfig['dbname']
        );

        if (!$this->connection) {
            throw new \RuntimeException('Database connection failed: ' . mysqli_connect_error());
        }

        mysqli_set_charset($this->connection, 'utf8mb4');
    }

    /**
     * Checks if a high score entry already exists for the given name, level and score
     */
    private function entryExists(string $name, int $level, int $score): bool {
        $sql = "SELECT 1 FROM high_scores 
                WHERE name = ? AND level = ? AND score = ? 
                LIMIT 1";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return false;
        }

        mysqli_stmt_bind_param($stmt, 'sii', $name, $level, $score);
        mysqli_stmt_execute($stmt);
        mysqli_stmt_store_result($stmt);

        $exists = mysqli_stmt_num_rows($stmt) > 0;
        mysqli_stmt_close($stmt);

        return $exists;
    }

    /**
     * Adds a new high score entry and returns it with the database ID
     *
     * @param HighScoreEntry $entry
     * @return HighScoreEntry|null Returns null if insertion fails or entry already exists
     */
    public function addHighScore(HighScoreEntry $entry): ?HighScoreEntry {
        $generatedHash = $entry->generateHash();

        if($generatedHash !== $entry->getHash()){
            return null;
        }

        // Check if entry already exists
        if ($this->entryExists($entry->getName(), $entry->getLevel(), $entry->getScore())) {
            return null;
        }

        $sql = "INSERT INTO high_scores (name, created_at, score, level) VALUES (?, ?, ?, ?)";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return null;
        }

        $name = $entry->getName();
        $createdAt = $entry->getCreatedAt()->format('Y-m-d H:i:s');
        $score = $entry->getScore();
        $level = $entry->getLevel();

        mysqli_stmt_bind_param($stmt, 'ssii', $name, $createdAt, $score, $level);

        $result = mysqli_stmt_execute($stmt);
        mysqli_stmt_close($stmt);

        if (!$result) {
            return null;
        }

        // Get the created entry from database to ensure we return exactly what was stored
        $insertId = mysqli_insert_id($this->connection);
        $sql = "SELECT name, created_at, score, level 
                FROM high_scores 
                WHERE id = ?";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return null;
        }

        mysqli_stmt_bind_param($stmt, 'i', $insertId);
        mysqli_stmt_execute($stmt);
        $result = mysqli_stmt_get_result($stmt);

        if ($row = mysqli_fetch_assoc($result)) {
            $createdEntry = new HighScoreEntry(
                $row['name'],
                new DateTime($row['created_at']),
                (int)$row['score'],
                (int)$row['level'],
                null
            );
            mysqli_stmt_close($stmt);
            return $createdEntry;
        }

        mysqli_stmt_close($stmt);
        return null;
    }


    /**
     * @param mysqli_result $result
     * @return array<HighScoreEntry>
     */
    private function convertResultToHighScores(mysqli_result $result): array {
        $highScores = [];
        while ($row = mysqli_fetch_assoc($result)) {
            $highScores[] = new HighScoreEntry(
                $row['name'],
                new DateTime($row['created_at']),
                (int)$row['score'],
                (int)$row['level'],
                null
            );
        }
        return $highScores;
    }


    /**
     * @return array<HighScoreEntry>
     */
    public function getTodayTopScores(): array {
        $sql = "SELECT name, created_at, score, level 
                FROM high_scores 
                WHERE DATE(created_at) = CURDATE()
                ORDER BY score DESC 
                LIMIT 20";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return [];
        }

        mysqli_stmt_execute($stmt);
        $result = mysqli_stmt_get_result($stmt);

        $highScores = $this->convertResultToHighScores($result);

        mysqli_stmt_close($stmt);

        return $highScores;
    }

    /**
     * @return array<HighScoreEntry>
     */
    public function getCurrentMonthTopScores(): array {
        $sql = "SELECT name, created_at, score, level 
                FROM high_scores 
                WHERE YEAR(created_at) = YEAR(CURRENT_DATE)
                AND MONTH(created_at) = MONTH(CURRENT_DATE)
                ORDER BY score DESC 
                LIMIT 20";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return [];
        }

        mysqli_stmt_execute($stmt);
        $result = mysqli_stmt_get_result($stmt);

        $highScores = $this->convertResultToHighScores($result);

        mysqli_stmt_close($stmt);

        return $highScores;
    }

    /**
     * @return array<HighScoreEntry>
     */
    public function getOverallTopScores(): array {
        $sql = "SELECT name, created_at, score, level 
                FROM high_scores 
                ORDER BY score DESC 
                LIMIT 20";

        $stmt = mysqli_prepare($this->connection, $sql);
        if (!$stmt) {
            return [];
        }

        mysqli_stmt_execute($stmt);
        $result = mysqli_stmt_get_result($stmt);
        $highScores = $this->convertResultToHighScores($result);
        mysqli_stmt_close($stmt);

        return $highScores;
    }

    public function __destruct() {
        if ($this->connection) {
            mysqli_close($this->connection);
        }
    }
}