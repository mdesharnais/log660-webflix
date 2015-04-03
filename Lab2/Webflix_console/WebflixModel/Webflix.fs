// Obtenez des informations sur F# via http://fsharp.net
// Voir le projet 'Didacticiel F#' pour obtenir de l'aide.
module Webflix

open System
open System.Data
open System.Data.Linq
open System.Data.Entity
open System.Data.SqlClient
open Microsoft.FSharp.Data.TypeProviders
open System.Collections.Generic
open System.Linq
open System.Text
open System.Threading.Tasks
open Microsoft.FSharp.Linq.NullableOperators

#if OracleInstalled
open Oracle.ManagedDataAccess.Client

//Connection in Webflix DB
[<Literal>]
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE38;PASSWORD=2F8U1IAA;"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

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
    Language : string option
    Length : int option
    Genres : string list
    Director : (Id * string) option
    Scenarists : (Id * string) list
    Actors : (Id * string * string list) list
    Summary : string option
    AverageRate : double option
    RecommendedFilms : string list
}

type ProfessionalDetails = {
    Id : Id
    FirstName : string
    LastName : string
    Birthdate : DateTime option
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
  let yearMin = Convert.ToDecimal yearMin
  let yearMax = Convert.ToDecimal yearMax

  let isSubstringIgnoreCase (s : string) (s' : string) =
    s'.ToLower().Contains(s.ToLower())

  query {
    for f in context.FILMS do
    where ((String.IsNullOrEmpty title || f.TITLE.ToLower().Contains(title.ToLower())) && 
           (yearMin = 0m || f.YEAR ?>= yearMin) && 
           (yearMax = 0m || f.YEAR ?<= yearMax) &&
           (countries.IsEmpty || f.COUNTRIES.Any(fun c -> countries.Contains c.NAME)) &&
           (String.IsNullOrEmpty(language) || (f.LANGUAGE.NAME = language)) &&
           (genres.IsEmpty || f.GENRES.Any(fun g -> genres.Contains g.NAME)) &&
           (String.IsNullOrEmpty(director) || (f.PROFESSIONAL.FIRST_NAME.ToLower() + " " + f.PROFESSIONAL.LAST_NAME.ToLower()).Contains(director.ToLower()) ) &&
           (actors.IsEmpty || f.PROFESSIONAL.FILMS_ROLES.Any(fun a -> actors.Contains (a.PROFESSIONAL.FIRST_NAME + " " + a.PROFESSIONAL.LAST_NAME))))
    select (f.ID, f.TITLE, f.YEAR)
  }
  |> Seq.toList
  |> List.map (fun (i: decimal, t, y : Nullable<decimal>) -> {Id = Convert.ToInt32 i; Title = t; Year = Convert.ToInt32 (y.GetValueOrDefault 0m)})
#else
    [ {Id = 1; Title = "La communauté des anneaux"; Year = 2001}
      {Id = 2; Title = "Les deux tours"; Year = 2002}
      {Id = 3; Title = "Le retour du roi"; Year = 2003}]
#endif
   
   
let queryFilmDetails (customerId : Id) (id : Id): FilmDetails option =
#if OracleInstalled
    let id = Convert.ToDecimal id
    let apfst f (x, y) = (f x, y)
    let apsnd f (x, y) = (x, f y)
    let context = EntityConnection.GetDataContext()
    let qs = (query { 
        for f in context.FILMS do
        where (f.ID = id)
        select f
    }
    |> Seq.toList)

    let averageRate = (query { 
        for fa in context.VIEW_FILMS_AVERAGE_RATE do
        where (fa.ID = id)
        select (fa.AVG_RATE)
    }
    |> Seq.exactlyOne)
    
    let actors = (query {
      for r in context.FILMS_ROLES do
      join p in context.PROFESSIONALS on (r.ID_PROFESSIONAL = p.ID)
      where (r.ID_FILM = id)
      select (r.ID_PROFESSIONAL, p.FIRST_NAME + " " + p.LAST_NAME)
      distinct
    }
    |> Seq.toList
    |> List.map (fun (actorId, name) ->
        (Convert.ToInt32 actorId, name, query {
          for r in context.FILMS_ROLES do
          where (r.ID_FILM = id && r.ID_PROFESSIONAL = actorId)
          select (r.CHARACTER)
        } |> Seq.toList)
      ))

    let nullStringToOption s = if String.IsNullOrEmpty s then None else Some s

    match qs with
    | [] -> None
    | [f] -> Some {
            FilmInfos = {Id = Convert.ToInt32 f.ID; Title = f.TITLE; Year = Convert.ToInt32 (f.YEAR.GetValueOrDefault 0m)}
            Countries = List.map (fun (x : EntityConnection.ServiceTypes.COUNTRy) -> x.NAME) (Seq.toList (f.COUNTRIES.AsEnumerable ()))
            Language = nullStringToOption f.LANGUAGE.NAME
            Length = if f.LENGTH_IN_MINUTES.HasValue then Some (Convert.ToInt32 f.LENGTH_IN_MINUTES.Value) else None
            Genres = List.map (fun (y : EntityConnection.ServiceTypes.GENRE) -> y.NAME) (Seq.toList (f.GENRES.AsEnumerable ()))
            Director = if f.ID_DIRECTOR.HasValue then Some (Convert.ToInt32 f.ID_DIRECTOR, f.PROFESSIONAL.FIRST_NAME + " " + f.PROFESSIONAL.LAST_NAME) else None
            Summary = nullStringToOption f.SUMMARY
            Scenarists = List.map (fun (p : EntityConnection.ServiceTypes.PROFESSIONAL) -> (Convert.ToInt32 p.ID, p.FIRST_NAME + " " + p.LAST_NAME)) (Seq.toList (f.PROFESSIONALS.AsEnumerable ()))
            Actors = actors
            AverageRate = if averageRate.HasValue then Some (Convert.ToDouble averageRate.Value) else None
            RecommendedFilms = recommendedFilms
        }
    | _ -> None
#else
    = None
#endif

let queryProfessionalDetails (id : Id) : ProfessionalDetails option = 
#if OracleInstalled
  let id = Convert.ToDecimal id
  let context = EntityConnection.GetDataContext()
  let qs = (query { 
    for p in context.PROFESSIONALS do
    where (p.ID = id)
    select p
  }
  |> Seq.toList)

  match qs with
    | [] -> None
    | [p] -> Some {
            Id = Convert.ToInt32 p.ID
            FirstName = if String.IsNullOrEmpty p.FIRST_NAME then "" else p.FIRST_NAME
            LastName = if String.IsNullOrEmpty p.LAST_NAME then "" else p.LAST_NAME
            Birthdate = if p.BIRTHDATE.HasValue then Some p.BIRTHDATE.Value else None
            Birthplace= if String.IsNullOrEmpty p.BIRTHPLACE then "" else p.BIRTHPLACE
            Biography= if String.IsNullOrEmpty p.BIOGRAPHY then "" else p.BIOGRAPHY.[..400]+"..."
        }
    | _ -> None
#else
    = None
#endif

let rent (c : CustomerInfo) (f : Id) = 
#if OracleInstalled
  try
    let nullable value = new System.Nullable<_>(value)
    let context = EntityConnection.GetDataContext()
    let fullContext = context.DataContext
    let dn = DateTime.Now
    let newRent =  new EntityConnection.ServiceTypes.RENTING(ID_CUSTOMER = (Convert.ToDecimal c), ID_FILM =  (Convert.ToDecimal f), RENT_DATE = dn)
    fullContext.AddObject("RENTINGS", newRent)
    fullContext.CommandTimeout <- nullable 1000
    let _ = fullContext.SaveChanges()
    //|> printfn "Saved changes: %d object(s) modified."
    None
  with | e -> Some e.Message
  
  (*
  let _ = context.RENTINGS

  let param1 = new OracleParameter("P_ID_CUSTOMER", OracleDbType.Int32, Convert.ToInt32 c,  ParameterDirection.Input);
  let param2 = new OracleParameter("P_ID_FILM", OracleDbType.Int32, Convert.ToInt32 f, ParameterDirection.Input);
  //let param3 = new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output);
  let _ = context.DataContext.ExecuteStoreCommand("BEGIN PROC_ADD_RENTING(:P_ID_CUSTOMER, :P_ID_FILM); end;", [param1; param2])
  let _ = context.DataContext.ExecuteFunction*)
#else
   Some "No database connected."
#endif
