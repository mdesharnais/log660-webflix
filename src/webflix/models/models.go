package main

import (
    "github.com/astaxie/beego/orm"
)

type Film struct {
	//Id					int8
	Title 				string
	NumberOfCopies		int8
	OriginalLanguage	string
	LengthMinutes		int8
	Country				*Country 'orm:"rel(many)"'
	Genre				*Genre 'orm:"rel(many)"'
	//Renting				*Renting 'orm:"rel(many)"'
	Director			*Director	'orm:"rel(one)"'
	Scenarist			*Scenarist	'orm:"rel(many)"'
	Actor				*Actor	'orm:"rel(many)"'
}

type Country struct {
	//Id					int8
	Name 				string
	//Film				*Film 'orm:"reverse(many)"'
}

type Genre struct {
	//Id					int8
	Name 				string
	//Film				*Film 'orm:"reverse(many)"'
}

type Renting struct {
	//Id					int8
	RentDate			time
	ReturnDate			time
	Film				*Film 'orm:"rel(one)"'
}

type Person struct {
	FirstName 			string
	LastName 			string
	Email 				string
	Phone				string
	Address				string
	Birthdate			time
	Password			string
}

const (// CreditCardType
	Visa = 0
	Mastercard = 1
	AmericanExpress = 2
)

type Customer struct {
	CreditCardType		int8
	CreditCardExpirationMonth int8
	CreditCardExpirationYear int8
	CreditCardCVV		string
	Person				*Person 'orm:"rel(one)"'
	Package				*Package 'orm:"rel(one)"'
}

type Employee struct {
	Number				int8
	Person				*Person 'orm:"rel(one)"'
}

type Package struct {
	Name				string
	CostPerMonth 		float32
	MaxFilms			int8
	MaxDays				int8
}

type Professional struct {
	FirstName 			string
	LastName 			string
	Birthdate			time
	Birthplace			string
	Biography			string
}

type Role struct {
	Professionnal		*Professionnal 'orm:"rel(one)"'	//actor
	Character			String
}

type Scenarist	struct {
	Professionnal		*Professionnal 'orm:"rel(one)"'
}

type Director	struct {
	Professionnal		*Professionnal 'orm:"rel(one)"'
}


func init() {
	
	orm.RegisterModel(...)
}