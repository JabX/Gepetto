namespace Gepetto.Web

open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module WordPage =

    let Main wordId =
        let result = Server.GetWord wordId |> Async.RunSynchronously
        let name = result.name |> System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase

        divAttr [attr.id "app"] [
            h3 [
                spanAttr [attr.``class`` "wordName"] [text <| name + " - "]
                spanAttr [attr.``class`` "wordType"] [text <| result.``type``]
            ]
            
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
                |> Array.map (fun syn -> li [aAttr [attr.href <| "/word/" + syn.id.ToString ()] [text syn.name]] :> Doc)
                |> ul
            ]
        ]