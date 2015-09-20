namespace Gepetto.Web

open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module WordPage =

    let Main wordId =
        divAttr [attr.id "app"] [
            h3 [text (Server.GetWord wordId |> Async.RunSynchronously)]
            
            divAttr [attr.``class`` "block"] [
                h4 [text "Les formes du mot"]
                Server.GetForms wordId
                |> Async.RunSynchronously
                |> Array.map (fun form -> li [text form] :> Doc)
                |> ul
            ]

            divAttr [attr.``class`` "block"] [
                h4 [text "Les synonymes du mot"]
                Server.GetSynonyms wordId
                |> Async.RunSynchronously
                |> Array.map (fun syn -> li [text syn] :> Doc)
                |> ul
            ]
        ]