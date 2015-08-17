namespace Gepetto.Web

open Gepetto
open WebSharper

module Server =
    [<Rpc>]
    let GetScore sentence =
        async { 
            return 
                match Score.Get sentence with
                | Score.Percentage x -> sprintf "%i%%" x
                | Score.Amount x -> (match x with
                                        | Score.HowMany.All  -> "Tout le monde"
                                        | Score.HowMany.Most -> "La plupart"
                                        | Score.HowMany.Some -> "Un certain nombre"
                                        | Score.HowMany.Few  -> "Quelques-uns"
                                        | _                  -> "Personne")
                | Score.Error -> "La question est mal formulée !"
        }