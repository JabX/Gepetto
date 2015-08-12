namespace Gepetto.Web

open Gepetto
open WebSharper

module Server =
    [<Rpc>]
    let GetScore sentence =
        async { return Score.Get sentence }