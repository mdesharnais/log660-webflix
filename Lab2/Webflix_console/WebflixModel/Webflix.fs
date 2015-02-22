// Obtenez des informations sur F# via http://fsharp.net
// Voir le projet 'Didacticiel F#' pour obtenir de l'aide.
module Webflix

open System
open System.Data
open System.Data.Linq
open System.Data.Entity
open Microsoft.FSharp.Data.TypeProviders
open System.Collections.Generic
open System.Linq
open System.Text
open System.Threading.Tasks

#if OracleInstalled
open Oracle.ManagedDataAccess.Client

//Connection in Webflix DB
[<Literal>]
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE23;PASSWORD=Jf51vmZi;"
//SqlEntityConnection Provider ="Oracle.DataAccess.Client   Oracle.ManagedDataAccess.Client"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

type private schema = SqlDataConnection<ConnectionString=cs, StoredProcedures = true>
#endif

type Id = int
type CustomerInfo = Id
type Year = int

type FilmInfos = {
    Id : Id
    Title : string
    Year : Year
}

type FilmDetails = {
    FilmInfos : FilmInfos
    Countries : string list
    Languages : string list
    Length : int
    Genres : string list
    Director : Id * string
    Scenarists : (Id * string) list
    Actors : (Id * string * string list) list
    Summary : string
}

type ProfessionalDetails = {
    Id : Id
    FirstName : string
    LastName : string
    Birthdate : DateTime
    Birthplace : string
    Biography : string
}

let connectCustomer (email : string) (password : string) : CustomerInfo option =
#if OracleInstalled
  let context = EntityConnection.GetDataContext()
  let cs = (query { 
    for c in context.CUSTOMERS do
    where (c.PERSON.EMAIL = email && c.PERSON.PASSWORD = password)
    select c
  }
  |> Seq.toList)

  match cs with
    | [] -> None
    | [c] -> Some (Convert.ToInt32 c.ID)
    | _ -> None
#else
    Some 1
#endif

let searchFilms
    (title : string)
    (yearMin : Year)
    (yearMax : Year)
    (countries : string list)
    (language : string)
    (genres : string list)
    (director : string)
    (actors : string list)
    : FilmInfos list =
#if OracleInstalled
  let context = EntityConnection.GetDataContext()
  // convert params in query for comparaison
  let cq = query { for c in countries do select c}    
  //let lq = query { for l in language do select l}
  let gq = query { for g in genres do select g}
  let aq = query { for a in actors do select a}

  query {
    for f in context.FILMS do
    join c in context.COUNTRIES
      on (c.FILMS)
    join l in context.LANGUAGES
      on (f.ID_LANGUAGE = l.ID) 
    join g in context.GENRES
      on (f.ID = g.ID)
    where (f.TITLE = title && 
           f.YEAR >= yearMin && 
           f.YEAR <= yearMax &&
           cq.Contains(c.NAME) && //Countries
           f.LANGUAGE.NAME = language && //Language
           f.GENRES.All && //Genres
           (f.PROFESSIONALS) && //director
           (f.PROFESSIONALS)) //actors  *)   
    select (f.ID, f.TITLE, f.YEAR)
  }
  |> Seq.toList
  |> List.map (fun (i: decimal, t, y : Nullable<decimal>) -> {Id = Convert.ToInt32 i; Title = t; Year = Convert.ToInt32 y.GetValueOrDefault})
#else
    [ {Id = 1; Title = "La communauté des anneaux"; Year = 2001}
      {Id = 2; Title = "Les deux tours"; Year = 2002}
      {Id = 3; Title = "Le retour du roi"; Year = 2003}]
#endif
   
let queryFilmDetails (id : Id) : FilmDetails option = None

let queryProfessionalDetails (id : Id) : ProfessionalDetails option = None

let rent (c : CustomerInfo) (f : Id) = Some "No database connected."