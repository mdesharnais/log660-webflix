open System
open Nessos.UnionArgParser
open Webflix

// --------------------------------------------------
// Utility types
// --------------------------------------------------

type SearchArguments =
    | Title of string
    | YearMin of int
    | YearMax of int
    | Country of string
    | Language of string
    | Genre of string
    | Director of string
    | Actor of string
with
    interface IArgParserTemplate with
        member s.Usage = ""


type Either<'a, 'b> =
    | Left of 'a
    | Right of 'b

// --------------------------------------------------
// Utility functions
// --------------------------------------------------

let lookupBy f x = List.tryPick (fun (k, v) -> if f k x then Some v else None)


let tryParseInt s =
    let (b, i) = System.Int32.TryParse s
    if b then Some i else None


let (>>=) (m : Either<'a,'b>) (f : 'b -> Either<'a,'c>) =
    match m with
    | Left(a) -> Left a
    | Right(b) -> f b


let either f g e =
    match e with
    | Left x -> f x
    | Right y -> g y


let matchOption n s o =
    match o with
    | None -> n
    | Some x -> s x


let extractSingleton xs =
    match xs with
    | [x] -> Some x
    | _ -> None

// --------------------------------------------------
// Core features
// --------------------------------------------------

let connect () : CustomerInfo option =
    try
        let line = IO.File.ReadAllText(".webflix", Text.Encoding.UTF8)
        match line.Split ' ' with
        | [|username; password|] ->
            match connectCustomer username password with
            | None ->  eprintfn "Invalid username or password."; None
            | Some id -> Some id
        | _ -> eprintf "Invalid configuration file."; None
    with
        | _ -> eprintf "Invalid configuration file."; None


let doConnect argv =
    match argv with
    | [username; password] ->
        IO.File.WriteAllText(".webflix", (username + " " + password), Text.Encoding.UTF8)
        matchOption -1 (fun _ -> 0) (connect ())
    | _ -> eprintfn "Invalid arguments. Syntax: webflix connect <email> <password>"; -1


let doDisconnect _ argv =
    match argv with
    | [] -> IO.File.Delete ".webflix"; 0
    | _ -> eprintfn "Invalid arguments. Syntax: webflix disconnect"; -1
    

let doSearch argv =
    let parser = UnionArgParser.Create<SearchArguments>()
    let sas = (parser.Parse (List.toArray argv)).GetAllResults ()

    let title = ref ""
    let yearMin = ref 0
    let yearMax = ref 0
    let countries = ref []
    let language = ref ""
    let genres = ref []
    let director = ref ""
    let actors = ref []
    
    let setFilters sa = 
        match sa with
        | Title s -> title := s
        | YearMin n -> yearMin := n
        | YearMax n -> yearMax := n
        | Country s -> countries := s :: !countries 
        | Language s -> language := s
        | Genre s -> genres := s :: !genres
        | Director s -> director := s
        | Actor s -> actors := s :: !actors

    List.iter setFilters sas

    match searchFilms !title !yearMin !yearMax !countries !language !genres !director !actors with
    | [] -> eprintf "No films founded."; -1
    | fs -> List.iter (fun (f : FilmInfos) -> printfn "%4d %4d %s" f.Id f.Year f.Title) fs; 0


let doShow argv =
    let formatList f = String.concat "\n" << List.map ((+) "  " << f)
    let formatOption f = matchOption "" (fun s -> f s)
    let intToString (n : int) = n.ToString ()
    let uncurry = (<||)

    let printFilmDetails f =
        printfn "Id: %d" f.FilmInfos.Id
        printfn "Title: %s" f.FilmInfos.Title
        printfn "Year: %d" f.FilmInfos.Year
        printfn "Countries:\n%s" (formatList id f.Countries)
        printfn "Language:\n%s" (formatOption id f.Language)
        printfn "Length (min.): %s" (formatOption intToString f.Length)
        printfn "Genres:\n%s" (formatList id f.Genres)
        printfn "Director: %s" (formatOption (uncurry (sprintf "[%d] %s")) f.Director)
        printfn "Scenarists:\n%s" (formatList (fun (id, s) -> sprintf "[%d] %s" id s) f.Scenarists)
        printfn "Actors:\n%s" (formatList (fun (id, s, rs) -> sprintf "[%d] %s (%s)" id s (String.concat ", " rs)) f.Actors)
        printfn "Summary: %s" (formatOption id f.Summary)

    let printProfessionalDetails p =
        printfn "Id: %d" p.Id
        printfn "Name: %s %s" p.FirstName p.LastName
        printfn "Birthdate: %s" (matchOption "" (fun (d : DateTime) -> d.ToShortDateString ()) p.Birthdate)
        printfn "Birthplace: %s" p.Birthplace
        printfn "Biography: %s" p.Biography
    
    match argv with
    | "professional" :: xs ->
        either (fun err -> eprintfn "%s" err; -1) (fun _ -> 0) (Right xs
            >>= (extractSingleton >> matchOption (Left "Invalid arguments. Syntax: webflix show professional <pro-id>") Right)
            >>= (tryParseInt >> matchOption (Left "<pro-id> must be an integer.") Right)
            >>= (Webflix.queryProfessionalDetails >> matchOption (Left "No professional correspond to this <pro-id>.") (fun f -> printProfessionalDetails f; Right 0)))
    | _ ->
        either (fun err -> eprintfn "%s" err; -1) (fun _ -> 0) (Right argv
            >>= (extractSingleton >> matchOption (Left "Invalid arguments. Syntax: webflix show <film-id>") Right)
            >>= (tryParseInt >> matchOption (Left "<film-id> must be an integer.") Right)
            >>= (Webflix.queryFilmDetails >> matchOption (Left "No film correspond to this <film-id>.") (fun f -> printFilmDetails f; Right 0)))


let doRent c argv =
    either (fun err -> eprintfn "%s" err; -1) (fun _ -> 0) (Right argv
        >>= (extractSingleton >> matchOption (Left "Invalid arguments. Syntax: webflix rent <film-id>") Right)
        >>= (tryParseInt >> matchOption (Left "<film-id> must be an integer.") Right)
        >>= (Webflix.rent c >> matchOption (Right ()) Left))

let doOnlyIfConnected f argv =
    match connect () with
    | None -> -1
    | Some c -> f c argv

// --------------------------------------------------
// Main
// --------------------------------------------------

[<EntryPoint>]
let main argv = 
    let cmds = [
        ("connect", doConnect)
        ("disconnect", doOnlyIfConnected doDisconnect)
        ("search", doSearch)
        ("show", doShow)
        ("rent", doOnlyIfConnected doRent)
        ]
    let usage = String.concat "\n" (List.map ((+) "  " << fst) cmds)

    match Array.toList argv with
    | x :: xs ->
        match lookupBy (=) x cmds with
        | Some f -> f xs
        | None -> eprintfn "Unknown command. Available commands:\n%s" usage; -1
    | _ -> eprintfn "No command provided. Available commands:\n%s" usage; -1