open Gepetto

module Program =
    [<EntryPoint>]
    let Main _ =
        while true do          
            printfn "Posez votre question !" 
            match System.Console.ReadLine () |> Score.Get with
            | Score.Percentage x -> printfn "%i%%" x
            | Score.Amount x -> (match x with
                                    | Score.HowMany.All  -> "Vraiment beaucoup"
                                    | Score.HowMany.Most -> "Une grosse majorité"
                                    | Score.HowMany.Some -> "Un certain nombre"
                                    | Score.HowMany.Few  -> "Quelques-uns"
                                    | _                  -> "Quasiment aucun")
                                |> printfn "%s"
            | Score.Error x -> (match x with
                                    | Score.ErrorType.Sentence     -> "La question est mal formulée !"
                                    | Score.ErrorType.QuestionType -> "Gepetto ne sait pas répondre à ça..."
                                    | _                            -> "Prout")
                                |> printfn "%s"
        0