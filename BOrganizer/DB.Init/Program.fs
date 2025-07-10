open System
open DB.Core
open DbUp
open Rechnungen.DB

let scriptProvider = [ DbScriptsProvider() ]

let connectionString =
    "Host=db;Port=5432;Username=pguser;Password=nice_pw;Database=invoices"

let getScriptAssembliesFromProviders () =
    AppDomain.CurrentDomain.GetAssemblies()
    |> Seq.collect (fun asm ->
        try
            asm.GetTypes()
        with _ ->
            [||]) // handle reflection exceptions safely
    |> Seq.filter (fun t -> t.IsClass && not t.IsAbstract && typeof<IDbScriptsProvider>.IsAssignableFrom(t))
    |> Seq.map (fun t ->
        let instance = Activator.CreateInstance(t) :?> IDbScriptsProvider
        instance.GetAssemblyWithScripts())
    |> Seq.concat



let runMigrations () =
    EnsureDatabase.For.PostgresqlDatabase(connectionString)

    let scriptAssemblies = getScriptAssembliesFromProviders ()
    let assemblies = scriptAssemblies |> Seq.toArray

    let upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssemblies(assemblies)
            .LogToConsole()
            .Build()

    let result = upgrader.PerformUpgrade()

    if result.Successful then
        printfn "Database upgraded successfully."
    else
        printfn "Database upgrade failed: %s" result.Error.Message
        Environment.Exit(1)

[<EntryPoint>]
let main _ =
    runMigrations ()
    0
