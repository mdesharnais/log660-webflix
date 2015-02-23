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
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE23;PASSWORD=Jf51vmZi;"
//SqlEntityConnection Provider ="Oracle.DataAccess.Client   Oracle.ManagedDataAccess.Client"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

//type private schema = SqlDataConnection<ConnectionString=cs, StoredProcedures = true>
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
  let yearMin = Convert.ToDecimal yearMin
  let yearMax = Convert.ToDecimal yearMax

  let isSubstringIgnoreCase (s : string) (s' : string) =
    s'.ToLower().Contains(s.ToLower())

  query {
    for f in context.FILMS do
    (*join c in context.COUNTRIES
      on (c.FILMS)
    join l in context.LANGUAGES
      on (f.ID_LANGUAGE = l.ID) 
    join g in context.GENRES
      on (f.ID = g.ID)*)
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
   
let queryFilmDetails (id : Id) : FilmDetails option = None
  (*let context = EntityConnection.GetDataContext()
  query { for f in context.FILMS do
          where f.ID = id
          select 
  }
  |> Seq.toList
  None*)

let queryProfessionalDetails (id : Id) : ProfessionalDetails option = None

let rent (c : CustomerInfo) (f : Id) = 
#if OracleInstalled

  let nullable value = new System.Nullable<_>(value)
  let context = EntityConnection.GetDataContext()
  let fullContext = context.DataContext
  let dn = DateTime.Now
  let newRent =  new EntityConnection.ServiceTypes.RENTING((*todo id*), ID_CUSTOMER = (Convert.ToDecimal c), ID_FILM =  (Convert.ToDecimal f), RENT_DATE = dn)
  fullContext.AddObject("RENTINGS", newRent)
  fullContext.CommandTimeout <- nullable 1000
  fullContext.SaveChanges()
  |> printfn "Saved changes: %d object(s) modified."
  
  
  (*
  let _ = context.RENTINGS

  let param1 = new OracleParameter("P_ID_CUSTOMER", OracleDbType.Int32, Convert.ToInt32 c,  ParameterDirection.Input);
  let param2 = new OracleParameter("P_ID_FILM", OracleDbType.Int32, Convert.ToInt32 f, ParameterDirection.Input);
  //let param3 = new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output);
  let _ = context.DataContext.ExecuteStoreCommand("BEGIN PROC_ADD_RENTING(:P_ID_CUSTOMER, :P_ID_FILM); end;", [param1; param2])
  let _ = context.DataContext.ExecuteFunction*)
  Some "Maybe it worked..."
#else
   Some "No database connected."
#endif
