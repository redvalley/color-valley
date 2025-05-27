CREATE TABLE high_scores (
                             id INT AUTO_INCREMENT PRIMARY KEY,
                             name VARCHAR(255) NOT NULL,
                             created_at DATETIME NOT NULL,
                             score INT NOT NULL,
                             level INT NOT NULL
);
