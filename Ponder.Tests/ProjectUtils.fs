module Ponder.Tests.ProjectUtils

open System.Xml.Linq
open System.IO
open Ponder.SlnTypes

let private inItemGroup x =
    let objs =
        x
        |> List.map (fun i -> i :> obj)
        |> Array.ofList
    XElement(
        "ItemGroup",
        objs
    )
    
let private reference (kind: string) inc =
    XElement(kind, XAttribute("Include", inc))
    

let csprojXmlFromProject (project: Project) =
    let containingDir = Path.GetDirectoryName project.Path
    let referenceElems =
        project.References
        |> List.map (fun x -> Path.GetRelativePath(containingDir, x))
        |> List.map (reference "ProjectReference")
        
    let testPackageRef =
        if project.IsTestProject
        then
            reference "PackageReference" "Microsoft.NET.Test.Sdk"
            |> List.singleton
            |> inItemGroup
            |> Option.Some
        else None
        
    let projectRefsItemGroup = 
        referenceElems
        |> inItemGroup
        
    let itemGroups =
        [
            Some projectRefsItemGroup;
            testPackageRef;
        ]
        |> List.choose id

    let doc =
        XDocument(
            XElement(
                "Project",
                itemGroups |> Array.ofList
            )
        )
    
    doc.ToString()
