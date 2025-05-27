<?php

class HighScoreEntry implements JsonSerializable {

    private static ?string $salt = null;


    public function __construct(
        private string $name,
        private DateTime $createdAt,
        private int $score,
        private int $level,
        private ?string $hash
    ) {}


    public function getName(): string {
        return $this->name;
    }

    public function setName(string $name): self {
        $this->name = $name;
        return $this;
    }

    public function getCreatedAt(): \DateTime {
        return $this->createdAt;
    }

    public function setCreatedAt(\DateTime $createdAt): self {
        $this->createdAt = $createdAt;
        return $this;
    }

    public function getScore(): int {
        return $this->score;
    }

    public function setScore(int $score): self {
        $this->score = $score;
        return $this;
    }

    public function getLevel(): int {
        return $this->level;
    }

    public function setLevel(int $level): self {
        $this->level = $level;
        return $this;
    }

    public function getHash(): string {
        return $this->hash;
    }

    public function setHash(string $hash): self {
        $this->hash = $hash;
        return $this;
    }

    /**
     * Get the currently configured salt
     *
     * @throws RuntimeException if salt is not configured
     */
    private static function getSalt(): string {
        if (self::$salt === null) {
            $config = require 'config.php';
            if (!isset($config['highscore']['hash_salt'])) {
                throw new RuntimeException('Hash salt is not configured');
            }
            self::$salt = $config['highscore']['hash_salt'];
        }
        return self::$salt;
    }

    /**
     * Generates a salted hash for this entry based on name, level, and score
     *
     * @throws RuntimeException if salt is not configured
     */
    public function generateHash(): string {
        $salt = self::getSalt();
        $data = sprintf('%s_%d_%d_%d', $this->name, $this->level, $this->score, $this->createdAt->getTimestamp());
        return hash_hmac('sha256', $data, $salt);
    }

    /**
     * Creates a HighScoreEntry object from JSON string
     *
     * @param string $json
     * @return self
     * @throws JsonException If JSON is invalid
     * @throws InvalidArgumentException If required fields are missing or invalid
     */
    public static function fromJson(string $json): self {
        try {
            $data = json_decode($json, true, 512, JSON_THROW_ON_ERROR);
        } catch (JsonException $e) {
            throw new JsonException('Invalid JSON format: ' . $e->getMessage());
        }

        // Validate required fields
        $requiredFields = ['name', 'score', 'level', 'created_at'];
        foreach ($requiredFields as $field) {
            if (!isset($data[$field])) {
                throw new InvalidArgumentException("Missing required field: {$field}");
            }
        }

        // Validate name
        if (empty($data['name']) || !is_string($data['name'])) {
            throw new InvalidArgumentException('Name must be a non-empty string');
        }

        // Validate hash
        if (empty($data['hash']) || !is_string($data['hash'])) {
            throw new InvalidArgumentException('Hash must be a non-empty string');
        }

        // Validate score
        if (!is_numeric($data['score']) || $data['score'] < 0) {
            throw new InvalidArgumentException('Score must be a non-negative number');
        }

        // Validate level
        if (!is_numeric($data['level']) || $data['level'] < 1) {
            throw new InvalidArgumentException('Level must be a positive number');
        }

        // Handle created_at field
        if (isset($data['created_at'])) {
            try {
                $createdAt = new DateTime($data['created_at']);
            } catch (Exception $e) {
                throw new InvalidArgumentException('Invalid date format for created_at');
            }
        } else {
            throw new InvalidArgumentException('CreatedAt was not specified');
        }

        return new self(
            name: $data['name'],
            createdAt: $createdAt,
            score: (int)$data['score'],
            level: (int)$data['level'],
            hash: $data['hash']
        );
    }

    public function jsonSerialize(): mixed {
        return [
            'name' => $this->name,
            'score' => $this->score,
            'level' => $this->level,
            'created_at' => $this->createdAt->format('c')
        ];
    }


}