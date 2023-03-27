[<Microsoft.FSharp.Core.AutoOpen>]
module Ponder.CliArgs

open System
open System.Text.RegularExpressions
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions
open Ponder.SlnTypes
open Serilog
open Serilog.Extensions.Logging

type CliArgs = {
    SlnFile: string option
    LogFactory: ILoggerFactory
}

let private flushInterval = TimeSpan.FromSeconds(0.25)
let private defaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"

let private logArgRegex = Regex("^--log=.+$", RegexOptions.IgnoreCase)

let private makeLoggerFactory logPath =
    let factory = new LoggerFactory()
    let logger =
        LoggerConfiguration().WriteTo
            .File(logPath)
            .CreateLogger()
    
    let provider = new SerilogLoggerProvider(logger);
        
    factory.AddProvider(provider);
    
    factory :> ILoggerFactory

let tryParseLogArg arg =
    if logArgRegex.IsMatch(arg)
    then
        let logPath = arg.Substring(arg.IndexOf("=") + 1)
        logPath
            |> makeLoggerFactory
            |> Some
    else
        None
        
let parse args =
    let logArgs, slnArgs =
        List.partition logArgRegex.IsMatch args
    
    {
        LogFactory =
            List.tryHead logArgs
            |> Option.map tryParseLogArg
            |> Option.flatten
            |> Option.defaultValue NullLoggerFactory.Instance
        SlnFile = List.tryHead slnArgs
    }
