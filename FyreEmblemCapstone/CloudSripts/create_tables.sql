-- DROP TABLE profile;
-- DROP TABLE save;
-- DROP TABLE game;

-- CREATE TABLE profile (
--     username VARCHAR(255) PRIMARY KEY,
--     data MEDIUMTEXT
-- );


CREATE TABLE save (
    id INT NOT NULL,
    username VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    data MEDIUMTEXT,
    PRIMARY KEY (username, id)
);


-- CREATE TABLE game (
--     id INT PRIMARY KEY AUTO_INCREMENT,
--     host VARCHAR(255) NOT NULL,
--     player VARCHAR(255) NOT NULL,
--     data MEDIUMTEXT
-- );