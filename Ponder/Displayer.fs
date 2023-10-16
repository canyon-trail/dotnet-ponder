module Ponder.Displayer

open System

let private displayState state =
    Console.Clear()
    printfn $"Got State %A{state}"

let createDisplayer stateStream =
    stateStream
    |> Observable.subscribe displayState
