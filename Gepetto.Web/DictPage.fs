namespace Gepetto.Web

open Gepetto.Dal

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
module DictPage =
    
    let resultLine result =
        li [
            aAttr [attr.href <| "/word/" + result.id.ToString ()] [
                text <| result.name + " (" + result.``type`` + ")"
            ]
        ] :> Doc

    let Main () =
        let searchTerm = Var.Create ""
        let searchResults = Var.Create Seq.empty<Word>

        let searchTermView = 
            searchTerm.View
            |> View.MapAsync (fun term -> 
                async {
                    if term.Length > 2 then
                        let! data = Server.SearchWordByName term
                        searchResults.Value <- data |> Array.toSeq
                    else searchResults.Value <- Seq.empty
                })

        let output (results: Var<seq<Word>>) =
            let block: (seq<Doc> -> Elt) = divAttr [attr.``class`` "block"]
            results.View
            |> View.Map (fun results ->
                match Seq.length results with
                | 0 -> block [h4 [text "Pas de résultats"]]
                | _ -> results |> Seq.map resultLine
                               |> ul :> Doc
                               |> Seq.singleton
                               |> Seq.append [h4 [text "Résultats"] :> Doc]
                               |> block)
            |> Doc.EmbedView
        
        divAttr [attr.id "app"] [
            h3 [text "Recherchez un mot..."]
            iAttr [attr.``class`` "fa fa-search fa-lg"] []
            Doc.Input [] searchTerm
            output searchResults
            searchTermView |> View.Map (fun () -> Doc.Empty) |> Doc.EmbedView
        ]