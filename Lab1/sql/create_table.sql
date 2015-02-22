
drop table rentings cascade constraints;
drop table films_countries cascade constraints;
drop table films_genres cascade constraints;
drop table films_roles cascade constraints;
drop table films_scenarists cascade constraints;
drop table professionals cascade constraints;
drop table genres cascade constraints;
drop table countries cascade constraints;
drop table films cascade constraints;
drop table languages cascade constraints;
drop table persons cascade constraints;
drop table employees cascade constraints;
drop table customers cascade constraints;
drop table packages cascade constraints;

create table languages(
  id integer primary key,
  name varchar(100) not null unique
);

create table professionals(
  id integer primary key,
  first_name varchar(100) not null,
  last_name varchar(100),
  birthdate date,
  birthplace varchar(100),
  biography varchar(4000)
);

create table films(
  id integer  primary key,
  title varchar(100) not null unique,
  year smallint,
  number_of_copies smallint not null,
  summary varchar(4000),
  length_in_minutes smallint,
  id_director integer references professionals(id),
  id_language integer references languages(id)
);

create table genres(
  id integer primary key,
  name varchar(100) not null unique
);

create table films_genres(
  id_film integer not null references films(id),
  id_genre integer not null references genres(id),
  constraint films_genres_pk primary key(id_film, id_genre)
);

create table countries(
  id integer primary key,
  name varchar(100) not null unique
);

create table films_countries(
  id_film integer not null references films(id),
  id_country integer not null references countries(id),
  constraint films_countries_pk primary key(id_film, id_country)
);

create table films_scenarists(
  id_film integer not null references films(id),
  id_professional integer not null references professionals(id),
  constraint films_scenarists_pk primary key(id_film, id_professional)
);

create table films_roles(
  id integer primary key,
  id_film integer not null references films(id),
  id_professional integer not null references professionals(id),
  character varchar(100) not null,
  constraint films_roles_un1 unique(id_film, id_professional, character)
);

create table persons(
  id integer primary key,
  first_name varchar(100) not null,
  last_name varchar(100) not null,
  email varchar(100) not null unique,
  telephone varchar(100) not null,
  address_civic_number varchar(10) not null,
  address_street varchar(100) not null,
  address_city varchar(100) not null,
  address_province smallint not null check(address_province between 0 and 10),
  -- enum province = AB 0 | BC 1 | MB 2 | NB 3 | NL 4 | NS 5 | ON 6 | PE 7 | QC 8 | SK 9
  address_postal_code varchar(6) not null,
  birthdate date not null,
  password varchar(20) not null check(regexp_like(password, '^[a-zA-Z0-9]*$') and length(password) >= 5) 
);

create table employees(
  id integer primary key references persons(id),
  emp_number number(7)
);

create table packages(
  id integer primary key,
  name varchar(100) not null unique,
  cost_per_month number(4,2) not null,
  max_films smallint not null,
  max_days smallint
);

create table customers(
  id integer primary key references persons(id),
  credit_card_number varchar(100) not null,
  credit_card_type smallint not null check(credit_card_type between 0 and 2),
  -- enum credit_card_type = VISA 0 | MASTER_CARD 1 | AMERICAN_EXPRESS 2
  credit_card_expiration_month smallint not null,
  credit_card_expiration_year smallint not null,
  credit_card_cvv varchar(3),
  id_package integer references packages(id)
  -- contraints
  -- credit card must not be expired
);

create table rentings(
  id integer primary key,
  id_customer integer not null references customers(id),
  id_film integer not null references films(id),
  rent_date date not null,
  return_date date
);

drop sequence seq_customers;
create sequence seq_customers
    start with 1
    increment by 1;
	
drop sequence seq_rentings;
create sequence seq_rentings
    start with 1
    increment by 1;

CREATE OR REPLACE VIEW films_available AS
SELECT films.title, films.number_of_copies AS total, count(rentings.id) as rented
FROM films
LEFT JOIN RENTINGS on films.id = rentings.id_film
GROUP BY films.title, films.number_of_copies;
