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
type FilmInfo = {Id : Id; title : string; year : Year}

let connectCustomer (email : string) (password : string) : CustomerInfo = 0

let searchFilms
    (title : string)
    (yearMin : Year)
    (yearMax : Year)
    (countries : string list)
    (language : string)
    (genres : string list)
    (director : string)
    (actors : string list)
    : FilmInfo list =
        [ {Id = 1; title = "La communauté des anneaux"; year = 2001}
        ; {Id = 2; title = "Les deux tours"; year = 2002}
        ; {Id = 3; title = "Le retour du roi"; year = 2003}]

let queryFilmDetails (id : Id) = None

let rent (c : CustomerInfo) (f : FilmInfo) = Some "No database connected."

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
