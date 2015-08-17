namespace Gepetto.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server
open WebSharper.UI.Next.Html

type EndPoint =
    | [<EndPoint "/">] Home

module Templating =
    type MainTemplate = Templating.Template<"Main.html">

    let Main ctx action title body =
        Content.Doc (
            MainTemplate.Doc (
                title = title,
                body = body
            )
        )

module Site =
    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Les chiffres de Gepetto" [
            client <@ Client.Main() @>
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
        )
