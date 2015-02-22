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
open Oracle.ManagedDataAccess.Client


//Connection in Webflix DB
[<Literal>]
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE23;PASSWORD=Jf51vmZi;"
//SqlEntityConnection Provider ="Oracle.DataAccess.Client   Oracle.ManagedDataAccess.Client"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

type private schema = SqlDataConnection<ConnectionString=cs, StoredProcedures = true>


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
    Director : string
    Scenarists : string list
    Actors : (string * string list) list
    Summary : string
}

//let connectCustomer (email : string) (password : string) : CustomerInfo = 0
let connectCustomer (email : string) (password : string) : CustomerInfo option =
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

   //   [ {Id = 1; title = "La communauté des anneaux"; year = 2001}
   //   ; {Id = 2; title = "Les deux tours"; year = 2002}
   //   ; {Id = 3; title = "Le retour du roi"; year = 2003}]

let queryFilmDetails (id : Id) : FilmDetails option = None
  (*let context = EntityConnection.GetDataContext()
  query { for f in context.FILMS do
          where f.ID = id
          select 
  }
  |> Seq.toList
  None*)


let rent (ci : CustomerInfo) (f : Id) = 
  let context = EntityConnection.GetDataContext()
  let foo = context.DataContext.ExecuteFunction("proc_add_renting", [("p_id_customer" * ])

  Some "No database connected."

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
