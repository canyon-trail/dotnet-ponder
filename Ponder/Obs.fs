module Ponder.Obs

open System

let private noneIfIsSame a b =
    if a = b then None
    else a
    
let distinct (stream: IObservable<'a>) =
    stream
    |> Observable.map Some
    |> Observable.scan noneIfIsSame None
    |> Observable.filter Option.isSome
    |> Observable.map Option.get
