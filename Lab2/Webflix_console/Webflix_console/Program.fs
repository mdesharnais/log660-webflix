(*
webflix connect "username" "password"
webflix search --title="foo" --year="2014" --lang="en" --lang="fr"
webflix show 1
webflix rent 1
*)

open Nessos.UnionArgParser



type SearchArguments =
    | Title of string
    | YearMin of int
    | YearMax of int
    | Countries of string
    | Language of string
    | Genres of string
    | Director of string
    | Actors of string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Title _ -> "specify a working directory."
            | YearMin _ -> "specify a listener (hostname : port)."
            | YearMax _ -> "binary data in base64 encoding."
            | Countries _ -> "specify a primary port."
            | Language _ -> "set the log level."
            | Genres _ -> "detach daemon from console."
            | Director _ -> "specify a primary port."
            | Actors _ -> "set the log level."

let show sa =
    match sa with
    | Title(s) -> s
    | YearMin(i) -> i.ToString ()
    | Language(s) -> s
    | _ -> "blablabla"

type Id = uint32
type CustomerInfo = Id
type Year = int
type FilmInfo = {Id : Id; title : string; year : Year}

let foo (sas : SearchArguments) : FilmInfo list = []

 
// build the argument parser
let parser = UnionArgParser.Create<SearchArguments>()
 
// get usage text
let usage = parser.Usage()
// output:
//    --working-directory <string>: specify a working directory.
//    --listener <host:string> <port:int>: specify a listener (hostname : port).
//    --log-level <int>: set the log level.
//    --detach: detach daemon from console.
//    --help [-h|/h|/help|/?]: display this list of options.

let doConnect argv =
    match argv with
    | username :: password -> printfn "connecting"; 0
    | _ -> printfn "Invalid arguments. Syntax: webflix connect <email> <password>"; -1

let doSearch argv =
    printfn "searching"
    let sas = (parser.Parse (List.toArray argv)).GetAllResults ()
    //let b = List.map show sas
    //let c = String.concat ", " b

    let mutable title : string = ""
    let mutable yearMin : Year = 0
    let mutable yearMax : Year = 0
    let mutable countries : string list = []
    let mutable language : string = ""
    let mutable genres : string list = []
    let mutable director : string = ""
    let mutable actors : string list = []
    
    let setFilters sa = 
        match sa with
        | Title(s) -> title <- s
        | YearMin(ymin) -> yearMin <- ymin
        | YearMax(ymax) -> yearMax <- ymax
        | Countries(c) -> countries <- c :: countries 
        | _ -> ()

    List.iter setFilters sas

let doShow argv =
    match argv with
    | id :: [] -> printfn "showing"; 0
    | _ -> printfn "Invalid arguments. Syntax: webflix show <film-id>"; -1

let doRent argv =
    match argv with
    | id :: [] -> printfn "renting"; 0
    | _ -> printfn "Invalid arguments. Syntax: webflix rent <film-id>"; -1

let doDebug argv =
    List.iter (printf "[%s]\n") argv; 0

[<EntryPoint>]
let main argv = 
    match Array.toList argv with
    | "connect" :: xs -> doConnect xs
    | "search" :: xs -> doSearch xs
    | "show" :: xs -> doShow xs
    | "rent" :: xs -> doRent xs
    | "debug" :: xs -> doDebug xs
    | _ -> printfn "Unknow command"; -1