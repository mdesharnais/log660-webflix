package main

import (
         "fmt"
         "io/ioutil"
         "os"
         "encoding/xml"
)

type Person struct {
	Nom 			    string      `xml:"nom"`
	Naissance           Naissance   `xml:"naissance"`
	Photo				string      `xml:"photo"`
	Bio				    string      `xml:"bio"`
}

type Naissance struct {
    Anniversaire        time    `xml:"anniversaire"`
    Lieu                string  `xml:"lieu"`
}

type InfoCredit struct {
    Carte               string  `xml:"carte"`
    No                  string  `xml:"no"`
    ExpMois             int8    `xml:"exp-mois"`
    ExpAnnee            int8    `xml:"exp-annee"`
}

type Client struct {
    NomFamille          string      `xml:"nom-famille"`
    Prenom              string      `xml:"prenom"`
    Courriel            string      `xml:"courriel"`
    Tel                 string      `xml:"tel"`
    InfoCredit          InfoCredit  `xml:"info-credit"`
    Anniversaire        time        `xml:"anniversaire"`
    Adresse             string      `xml:"annee"`
    Ville               string      `xml:"ville"`
    Province            string      `xml:"province"`
    CodePostal          string      `xml:"code-postal"`
    MotDePasse          string      `xml:"mot-de-passe"`
    Forfait             string      `xml:"forfait"`
}

type Role struct {
    Acteur              string  `xml:"acteur"`
    Personnage          string  `xml:"personnage"`
}

type Film struct {
    XMLName             xml.Name `xml:"film"`
    Titre               string   `xml:"titre"`
    Annee               int8     `xml:"annee"`
    Pays                string   `xml:"pays"`
    Langue              string   `xml:"langue"`
    Duree               int8     `xml:"duree"`
    Resume              string   `xml:"resume"`
    Genres              []string `xml:"genre"`
    Realisateur         string   `xml:"realisateur"`
    Scenaristes         []string `xml:"scenariste"`
    Roles               []Role   `xml:"role"`
    Poster              string   `xml:"poster"`
}

func main() {
     xmlFile, err := os.Open("personnes_latin1.xml")
     if err != nil {
             fmt.Println("Error opening file:", err)
             return
     }
     defer xmlFile.Close()

     XMLdata, _ := ioutil.ReadAll(xmlFile)

     var c Company
     xml.Unmarshal(XMLdata, &c)

     fmt.Println(c.Staffs)
}

