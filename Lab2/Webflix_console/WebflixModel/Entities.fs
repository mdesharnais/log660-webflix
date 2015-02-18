module WebflixModel

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

[<Literal>]
let cs = @"DATA SOURCE=big-data-3.logti.etsmtl.ca/LOG660;PERSIST SECURITY INFO=True;USER ID=EQUIPE23;PASSWORD=Jf51vmZi;"
  
//SqlEntityConnection Provider ="Oracle.DataAccess.Client   Oracle.ManagedDataAccess.Client"
type private EntityConnection = SqlEntityConnection<ConnectionString=cs, Provider = "Oracle.ManagedDataAccess.Client", Pluralize = true>

  let context = EntityConnection.GetDataContext()  

  (*[<EntryPoint>]
  let main argv = 
    let context = EntityConnection.GetDataContext()
    let c = context.COUNTRIES

    query { for c in context.FILMS do
            select (c.TITLE, c.YEAR)
    }
    |> Seq.iter(fun (title, year) -> printfn "%s %s" title (year.ToString()) )
    0
  *) 