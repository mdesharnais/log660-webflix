
CREATE TABLE etl_time
(
  id_time       INT     PRIMARY KEY,
  the_date      DATE    NOT NULL,
  the_hour      INT     NOT NULL,
  the_year      INT     NOT NULL,
  day_of_week   VARCHAR(50) NOT NULL,
  month_of_year VARCHAR(50) NOT NULL
);

CREATE TABLE etl_film
(
  id_film              INTEGER  PRIMARY KEY,
  titre                VARCHAR(200)  NOT NULL,
  year                 INTEGER  NOT NULL,
  origin               VARCHAR(50)  NOT NULL,
  is_genre_action      SMALLINT NOT NULL,
  is_genre_adventure   SMALLINT NOT NULL,
  is_genre_animation   SMALLINT NOT NULL,
  is_genre_biography   SMALLINT NOT NULL,
  is_genre_comedy      SMALLINT NOT NULL,
  is_genre_crime       SMALLINT NOT NULL,
  is_genre_documentary SMALLINT NOT NULL,
  is_genre_drama       SMALLINT NOT NULL,
  is_genre_family      SMALLINT NOT NULL,
  is_genre_fantasy     SMALLINT NOT NULL,
  is_genre_film_noir   SMALLINT NOT NULL,
  is_genre_history     SMALLINT NOT NULL,
  is_genre_horror      SMALLINT NOT NULL,
  is_genre_music       SMALLINT NOT NULL,
  is_genre_musical     SMALLINT NOT NULL,
  is_genre_mystery     SMALLINT NOT NULL,
  is_genre_romance     SMALLINT NOT NULL,
  is_genre_scifi       SMALLINT NOT NULL,
  is_genre_sport       SMALLINT NOT NULL,
  is_genre_thriller    SMALLINT NOT NULL,
  is_genre_war         SMALLINT NOT NULL,
  is_genre_western     SMALLINT NOT NULL
);

CREATE TABLE etl_customer
(
  id_customer INTEGER PRIMARY KEY,
  name        VARCHAR(100) NOT NULL,
  age_group   VARCHAR(50) NOT NULL,
  oldness     INTEGER NOT NULL,
  postal_code VARCHAR(50) NOT NULL,
  city        VARCHAR(50) NOT NULL,
  province    VARCHAR(50) NOT NULL
);

CREATE TABLE etl_renting
(
  id_customer INT NOT NULL references customer(id_customer),
  id_film     INT NOT NULL references film(id_film),
  id_time     INT NOT NULL references time(id_time)
);
