/**/
drop table rentings;
drop table films_countries;
drop table films_genres;
drop table films_scenarists;
drop table films_actors;
drop table films;
drop table genres;
drop table countries;
drop table languages;
drop table professionals;
drop table employees;
drop table customers;
drop table packages;
drop table persons;

/**/

create table languages(
  id integer primary key,
  name varchar(100) not null unique
);

create table films(
  id integer  primary key,
  title varchar(100) not null unique,
  year smallint,
  number_of_copies smallint not null,
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

create table professionals(
  id integer primary key,
  first_name varchar(100) not null,
  last_name varchar(100) not null,
  birthdate date,
  birthplace varchar(100),
  biography varchar(4000)
);

create table films_scenarists(
  id_film integer not null references films(id),
  id_professionnal integer not null references professionals(id),
  constraint films_scenarists_pk primary key(id_film, id_professionnal)
);

create table films_actors(
  id integer primary key,
  id_film integer not null references films(id),
  id_professionnal integer not null references professionals(id),
  character varchar(100) not null,
  constraint films_actors_un1 unique(id_film, id_professionnal, character)
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
  address_province smallint not null check(address_province between 0 and 9),
  -- enum province = AB 0 | BC 1 | MB 2 | NB 3 | NL 4 | NS 5 | ON 6 | PE 7 | QC 8 | SK 9
  address_postal_code varchar(6) not null,
  birthdate date not null,
  password raw(32) not null
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
  credit_card_cvv varchar(3) not null,
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

create or replace sequence seq_customers
    start with 1
    increment by 1;

create or replace procedure proc_add_customer(
    p_credit_card_number varchar,
    p_credit_card_type smallint,
    p_credit_card_expiration_month smallint,
    p_credit_card_expiration_year smallint,
    p_credit_card_cvv varchar,
    p_id_package integer)
    is
    begin
        insert into customers 
            (id,
            credit_card_number,
            credit_card_type,
            credit_card_expiration_month,
            credit_card_expiration_year,
            credit_card_cvv,
            id_package)
        values
            (customers_seq.nextval,
            p_credit_card_number,
            p_credit_card_type,
            p_credit_card_expiration_month,
            p_credit_card_expiration_year,
            p_credit_card_cvv,
            p_id_package)
    end p_add_customer;

create or replace sequence seq_rentings
    start with 1
    increment by 1;

create or replace procedure proc_add_rentings(
    p_id integer,
    p_id_customer integer,
    p_id_film integer,
    p_rent_date date,
    p_return_date date)
    is
    begin
        insert into rentings
            (id integer,
            id_customer integer,
            id_film integer,
            rent_date date,
            return_date date)
        values
            (seq_rentings.nextval,
            p_id_customer integer,
            p_id_film integer,
            p_rent_date date,
            p_return_date date)        
    end proc_add_rental;
