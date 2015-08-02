namespace Gepetto

open SharpNL
open SharpNL.Analyzer
open FSharp.Data

module Main =
    let analyzer = new AggregateAnalyzer ()
    analyzer.Add "../../pos-model/fr-sent.bin"
    analyzer.Add "../../pos-model/fr-token.bin"
    analyzer.Add "../../pos-model/fr-pos-maxent.bin"

    type WordType = SqlEnumProvider<"select typeId from WordType", Config.DbString>
    type GScores = SqlCommandProvider<"select gScore from getScores(@data) order by wRank", Config.DbString>
    type InputWord = GScores.InputWord

    [<EntryPoint>]
    let main _ =
        while true do          
            printfn "Posez votre question !" 

            // Gets the input sentence and gets it through a POS tagger
            let doc = new Document ("fr", System.Console.ReadLine ())
            analyzer.Analyze doc

            // Keeping the word order in that index
            let mutable index = 0

            // Tags are in the Tokens of the first Sentence of the Document
            let words = (doc.Sentences |> Seq.head).Tokens 
                        |> Seq.map (fun token ->
                            // Matching the tags with the types from the DB
                            let dbToken = match token.POSTag with
                                          | "A" -> WordType.adj
                                          | "V" -> WordType.vrb
                                          | "N" -> WordType.nom
                                          | "ADV" -> WordType.adv
                                          | "D" -> WordType.art
                                          | "PRO" | "CL" -> WordType.pnm
                                          | "CC" -> WordType.cjc
                                          | "P" -> WordType.pps
                                          | "PREF" -> WordType.pfx
                                          | "I" -> WordType.itj
                                          | x -> x
                            (token.Lexeme, dbToken)) // Lexeme is the actual word
                        |> Seq.filter (fun x -> WordType.TryParse(snd x).IsSome) // Removes words that aren't tagged
                        |> Seq.map (fun x -> index <- index + 1
                                             new InputWord (index, fst x, snd x))
            let score = (new GScores ()).Execute(words) |> Seq.sum // For now, we're just doing the sum of all scores
            printfn "%i%%" (score % 10 + 75) // Well, that's pretty basic too
        0

