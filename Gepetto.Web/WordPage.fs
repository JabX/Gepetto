namespace Gepetto.Web

open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module WordPage =

    let Main wordId =
        div [
            aAttr [attr.href "/"] [text "Les chiffres"]
            aAttr [attr.href "/search"] [text "Le dictionnaire"]
            h2 [text (Server.GetWord wordId |> Async.RunSynchronously)]
            
            h4 [text "Les formes du mot"]
            Server.GetForms wordId
            |> Async.RunSynchronously
            |> Array.map (fun form -> li [text form] :> Doc)
            |> ul

            h4 [text "Les synonymes du mot"]
            Server.GetSynonyms wordId
            |> Async.RunSynchronously
            |> Array.map (fun syn -> li [text syn] :> Doc)
            |> ul
        ]