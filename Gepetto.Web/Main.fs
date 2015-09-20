namespace Gepetto.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server
open WebSharper.UI.Next.Html

module Templating =
    type MainTemplate = Templating.Template<"Main.html">

    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
             liAttr [if endpoint = act then yield attr.``class`` "active"] [
                aAttr [attr.href (ctx.Link act)] [text txt]
             ]
        [
            "chiffres" => EndPoint.Home
            "dictionnaire" => EndPoint.Search
            li [aAttr [attr.href "http://www.github.com/JabX/gepetto"] [text "github"]]
        ]

    let Main ctx action body =
        Content.Doc (
            MainTemplate.Doc (
                menubar = MenuBar ctx action,
                body = body
            )
        )

module Site =
    let Home ctx =
        Templating.Main ctx EndPoint.Home [
            client <@ HomePage.Main () @>
        ]

    let Search ctx =
        Templating.Main ctx EndPoint.Search [
            client <@ DictPage.Main () @>
        ]

    let Word ctx wordId =
        Templating.Main ctx EndPoint.Search [
            WordPage.Main wordId
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> Home ctx
            | EndPoint.Search -> Search ctx
            | EndPoint.Word wordId-> Word ctx wordId
        )
