namespace Gepetto.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =
    let Main () =
        let input = inputAttr [attr.value ""] []
        let output = h1 []
        div [
            input
            buttonAttr [
                on.click (fun _ _ ->
                    async {
                        let! data = Server.GetScore input.Value
                        output.Text <- sprintf "%i%%" data
                    }
                    |> Async.Start
                )
            ] [text "Poser la question !"]
            h4 [text "La réponse :"]
            div [output]
        ]
