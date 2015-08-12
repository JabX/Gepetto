namespace Gepetto

module Program =
    [<EntryPoint>]
    let Main _ =
        while true do          
            printfn "Posez votre question !" 
            System.Console.ReadLine () 
            |> Score.Get
            |> printfn "%i%%"
        0

