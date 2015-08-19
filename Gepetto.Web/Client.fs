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
        let input = inputAttr [attr.value ""; attr.name "gepetto"] []
        let answer = h1 []
        let status = Var.Create Empty

        let output (st: Var<Status>) =
            let repDiv = divAttr [attr.id "response"]
            View.FromVar st
            |> View.Map (fun stat -> 
                match stat with
                | Empty -> repDiv []
                | Working -> repDiv [ h4 [text "Recherche en cours..."] ]
                | Done -> repDiv [
                            h4 [text "La réponse :"]
                            answer
                        ])
            |> Doc.EmbedView
            
        divAttr [attr.id "app"] [
            h2 [text "Les chiffres de Gepetto"]
            form [
                input
                buttonAttr [
                    attr.``type`` "submit"
                    attr.formaction "javascript:this.click()"
                    on.click (fun _ _ ->
                        status.Value <- Working
                        async {
                            let! data = Server.GetScore input.Value
                            answer.Text <- data
                            status.Value <- Done
                        }
                        |> Async.Start
                    )
                ] [text "Demander à Gepetto !"]
            ]
            output status
        ]