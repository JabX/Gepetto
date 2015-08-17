namespace Gepetto.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =
    type Status = 
        | Empty
        | Working
        | Done

    let Main () =
        let input = inputAttr [attr.value ""] []
        let answer = h1 []
        let status = Var.Create Empty

        let output (st: Var<Status>) =
            View.FromVar st
            |> View.Map (fun stat -> 
                match stat with
                | Empty -> div []
                | Working -> h4 [text "Recherche en cours..."]
                | Done -> div [
                            h4 [text "La réponse :"]
                            div [answer]
                        ])
            |> Doc.EmbedView
            
        div [
            h2 [text "Les chiffres de Gepetto"]
            input
            buttonAttr [
                on.click (fun _ _ ->
                    status.Value <- Working
                    async {
                        let! data = Server.GetScore input.Value
                        answer.Text <- sprintf "%i%%" data
                        status.Value <- Done
                    }
                    |> Async.Start
                )
            ] [text "Poser la question !"]
            output status
        ]