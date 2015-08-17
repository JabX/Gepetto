open Gepetto

module Program =
    [<EntryPoint>]
    let Main _ =
        while true do          
            printfn "Posez votre question !" 
            match System.Console.ReadLine () |> Score.Get with
            | Score.Percentage x -> printfn "%i%%" x
            | Score.Amount x -> (match x with
                                 | Score.HowMany.All  -> "Tout le monde"
                                 | Score.HowMany.Most -> "La plupart"
                                 | Score.HowMany.Some -> "Un certain nombre"
                                 | Score.HowMany.Few  -> "Quelques-uns"
                                 | _                  -> "Personne")
                                |> printfn "%s"
            | Score.Error -> printfn "La question est mal formulée !"
        0