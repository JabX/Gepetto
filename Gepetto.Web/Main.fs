namespace Gepetto.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server
open WebSharper.UI.Next.Html

module Templating =
    type MainTemplate = Templating.Template<"Main.html">

    let Main title body =
        Content.Doc (
            MainTemplate.Doc (
                title = title,
                body = body
            )
        )

module Site =
    let Home ctx =
        Templating.Main "Les chiffres de Gepetto" [
            client <@ HomePage.Main () @>
        ]

    let Search ctx =
        Templating.Main "Les chiffres de Gepetto" [
            client <@ SearchPage.Main () @>
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> Home ctx
            | EndPoint.Search -> Search ctx
        )
