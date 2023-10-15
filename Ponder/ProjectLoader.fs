module Ponder.ProjectLoader

open System
open System.IO
open System.Xml.Linq
open Microsoft.Extensions.Logging
open Ponder.SlnTypes
open Ponder.AppState.States
open Ponder.AppState.Actions
open Ponder.AppStateReactor
open Ponder.Filesystem

let private isOpening state =
    match state with
    | Opening _ -> true
    | _ -> false
    
    
let private loadProject (filesystem: IFilesystem) (project: SlnProject) = async {
    let! lines = filesystem.LoadFile project.Path
    let contents = String.Join(Environment.NewLine, lines)
    
    let root = XElement.Parse contents
    
    let projectFolder = Path.GetDirectoryName project.Path
    
    let otherProjects =
        root.Descendants("ProjectReference")
        |> Seq.map (fun x -> x.Attribute("Include") |> Option.ofObj)
        |> Seq.filter Option.isSome
        |> Seq.map Option.get
        |> Seq.map (fun x -> x.Value)
        |> Seq.map (fun x -> Path.GetFullPath(Path.Combine(projectFolder, x)))
        |> List.ofSeq
        
    let isTest =
        root.Descendants("PackageReference")
        |> Seq.map (fun x -> x.Attribute("Include") |> Option.ofObj)
        |> Seq.filter Option.isSome
        |> Seq.map Option.get
        |> Seq.map (fun x -> x.Value)
        |> Seq.filter (fun x -> x = "Microsoft.NET.Test.Sdk")
        |> Seq.isEmpty
        |> not
        
    return {
        Name = project.Name;
        Path = project.Path;
        References = otherProjects;
        IsTestProject = isTest;
    }
}
    
let private loadProjects filesystem (dispatch: Dispatch) (logger: ILogger) state =
    logger.LogInformation("inside loadProjects")
    let slnFile =
        match state with
        | Opening sln -> sln
        | _ -> failwith "Expected Opening state"
        
    let projectAsyncs =
        slnFile.Projects
        |> List.map (loadProject filesystem)
        
    async {
        let! projectsArray =
            projectAsyncs
            |> Async.Parallel

        let projects =
            projectsArray
            |> List.ofSeq
            
        do dispatch (projects |> ProjectsLoaded)
        
        return ()
    }
    |> Async.RunSynchronously
    
    ()
    
let createLoader (reactor: Reactor) (filesystem: IFilesystem) (logger: ILogger): IDisposable =
    let relevantEvents =
        reactor.Stream
        |> Observable.filter isOpening

    relevantEvents
    |> Observable.subscribe (loadProjects filesystem reactor.Dispatch logger)
