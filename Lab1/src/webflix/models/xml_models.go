package main

import (
         "fmt"
         "io/ioutil"
         "os"
         "encoding/xml"
)

type Persons struct {
    XMLName xml.Name `xml:"personnes"`
    Persons []Person `xml:"personne"`
}

type Person struct {
    XMLName xml.Name                `xml:"personne"`
	Nom 			    string      `xml:"nom"`
	Naissance           Naissance   `xml:"naissance"`
	Photo				string      `xml:"photo"`
	Bio				    string      `xml:"bio"`
}

type Naissance struct {
    XMLName xml.Name            `xml:"naissance"`
    Anniversaire        string  `xml:"anniversaire"`
    Lieu                string  `xml:"lieu"`
}

type InfoCredit struct {
    XMLName xml.Name            `xml:"info-credit"`
    Carte               string  `xml:"carte"`
    No                  string  `xml:"no"`
    ExpMois             string  `xml:"exp-mois"`
    ExpAnnee            string  `xml:"exp-annee"`
}

type Clients struct {
    XMLName xml.Name  `xml:"clients"`
    Clients []Client  `xml:"client"`
}

type Client struct {
    XMLName xml.Name                `xml:"client"`
    NomFamille          string      `xml:"nom-famille"`
    Prenom              string      `xml:"prenom"`
    Courriel            string      `xml:"courriel"`
    Tel                 string      `xml:"tel"`
    InfoCredit          InfoCredit  `xml:"info-credit"`
    Anniversaire        string      `xml:"anniversaire"`
    Adresse             string      `xml:"annee"`
    Ville               string      `xml:"ville"`
    Province            string      `xml:"province"`
    CodePostal          string      `xml:"code-postal"`
    MotDePasse          string      `xml:"mot-de-passe"`
    Forfait             string      `xml:"forfait"`
}

type Role struct {
    XMLName xml.Name            `xml:"role"`
    Acteur              string  `xml:"acteur"`
    Personnage          string  `xml:"personnage"`
}

type Films struct {
    XMLName xml.Name  `xml:"films"`
    Films []Film      `xml:"film"`
}

type Film struct {
    XMLName             xml.Name `xml:"film"`
    Titre               string   `xml:"titre"`
    Annee               string   `xml:"annee"`
    Pays                string   `xml:"pays"`
    Langue              string   `xml:"langue"`
    Duree               string   `xml:"duree"`
    Resume              string   `xml:"resume"`
    Genres              []string `xml:"genre"`
    Realisateur         string   `xml:"realisateur"`
    Scenaristes         []string `xml:"scenariste"`
    Roles               []Role   `xml:"role"`
    Poster              string   `xml:"poster"`
}

func (p Person) String() string {
         return fmt.Sprintf("\t Nom : %s Naissance: %s \n", p.Nom, p.Naissance.Anniversaire)
 }
 
func (f Film) String() string {
     return fmt.Sprintf("\t Titre : %s \n", f.Titre)
}

func (c Client) String() string {
     return fmt.Sprintf("\t NomFamille : %s \n", c.NomFamille)
}

func main() {
 readPersonnes()
  
}

func readPersonnes() {
    xmlFile, err := os.Open("personnes.xml")
     if err != nil {
             fmt.Println("Error opening file:", err)
             return
     }
     defer xmlFile.Close()

     XMLdata, _ := ioutil.ReadAll(xmlFile)

     var persons Persons
     xml.Unmarshal(XMLdata, &persons)

     fmt.Println("\t nb personnes : ", len(persons.Persons))
     fmt.Println(persons.Persons)
}

func readFilms() {
    xmlFile, err := os.Open("films.xml")
    if err != nil {
        fmt.Println("Error opening file:", err)
        return
    }
    defer xmlFile.Close()

    XMLdata, _ := ioutil.ReadAll(xmlFile)

    var films Films
    xml.Unmarshal(XMLdata, &films)

    fmt.Println("\t nb films : ", len(films.Films))
    fmt.Println(films.Films)
}

func readClients() {
    xmlFile, err := os.Open("clients.xml")
    if err != nil {
        fmt.Println("Error opening file:", err)
        return
    }
    
    defer xmlFile.Close()

    XMLdata, _ := ioutil.ReadAll(xmlFile)

    var clients Clients
    xml.Unmarshal(XMLdata, &clients)

    fmt.Println("\t nb clients : ", len(clients.Clients))
    fmt.Println(clients.Clients)
}

