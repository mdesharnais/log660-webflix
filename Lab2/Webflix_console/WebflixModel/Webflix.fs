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
//open Oracle.ManagedDataAccess.Client

type Id = int
type CustomerInfo = Id
type Year = int

type FilmInfos = {
    Id : Id
    Title : string
    Year : Year
}

type FilmDetails= {
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

let connectCustomer (email : string) (password : string) : CustomerInfo option = Some 1

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
        [ {Id = 1; Title = "La communauté des anneaux"; Year = 2001}
        ; {Id = 2; Title = "Les deux tours"; Year = 2002}
        ; {Id = 3; Title = "Le retour du roi"; Year = 2003}]

let queryFilmDetails (id : Id) : FilmDetails option = None
let queryProfessionalDetails (id : Id) : ProfessionalDetails option = None

let rent (c : CustomerInfo) (f : Id) = Some "No database connected."

(*
[<Literal>]
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE23;PASSWORD=Jf51vmZi;"
//SqlEntityConnection Provider ="Oracle.DataAccess.Client   Oracle.ManagedDataAccess.Client"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

let context = EntityConnection.GetDataContext()

let query1 = query { for f in context.FILMS do
                        select (f.COUNTRIES, f.TITLE)
               }
query1
|> Seq.iter (fun (cs, ft) ->
   let ns = Seq.map (fun (c : EntityConnection.ServiceTypes.COUNTRy) -> c.NAME) cs
   //let _ = Seq.iter (fun c -> c.NAME) (cs.ToList())
   printfn "%s %s" (String.concat ", " ns) ft)
 // ((String.concat ", " ns) ft))

0
*)
(*
[<EntryPoint>]
let webflix = 
  //let c = context.COUNTRIES
  (*
  query { for c in context.FILMS do
          select (c.TITLE, c.YEAR)
  }
  |> Seq.iter(fun (title, year) -> printfn "%s %s" title (year.ToString()) )
  *)
  //let f = context.FILMS
  let query1 = query { for f in context.FILMS do
                        select (f.COUNTRIES, f.TITLE)
               }
  query1
  |> Seq.iter (fun (cs, ft) ->
    let ns = Seq.map (fun (c : EntityConnection.ServiceTypes.COUNTRy) -> c.NAME) cs
    //let _ = Seq.iter (fun c -> c.NAME) (cs.ToList())
    //printfn "%s %s" (String.concat ", " ns) ft)
    (String.concat ", " ns) ft)

 // System.Diagnostics.Debug.WriteLine(query1)
  0
  *)
