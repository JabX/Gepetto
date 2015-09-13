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
                                        | Score.HowMany.All  -> "Vraiment beaucoup"
                                        | Score.HowMany.Most -> "Une grosse majorité"
                                        | Score.HowMany.Some -> "Un certain nombre"
                                        | Score.HowMany.Few  -> "Quelques-uns"
                                        | _                  -> "Quasiment aucun")
                | Score.Error x -> (match x with
                                    | Score.ErrorType.Sentence     -> "La question est mal formulée !"
                                    | Score.ErrorType.QuestionType -> "Gepetto ne sait pas répondre à ça..."
                                    | _                            -> "Prout")
        }

    [<Rpc>]
    let SearchWordByName (word: string) =
        async { 
            return Dal.SearchWordByName word |> Seq.toArray
        }