namespace Gepetto

open SharpNL
open SharpNL.Analyzer
open FSharp.Data

module Score =
    let analyzer = new AggregateAnalyzer ()
    analyzer.Add "../../pos-model/fr-sent.bin"
    analyzer.Add "../../pos-model/fr-token.bin"
    analyzer.Add "../../pos-model/fr-pos-maxent.bin"

    type WordType = SqlEnumProvider<"select typeId from WordType", Config.DbString>
    type GScores = SqlCommandProvider<"select gScore from getScores(@data) order by wRank", Config.DbString>
    type InputWord = GScores.InputWord

    /// <summary>Computes the score from the given sentence and returns it</summary>
    let Get (sentence: string) =
        // We're using a POS-Tagger to tag the sentence
        let doc = new Document ("fr", sentence)
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
                                         new InputWord (index, fst x, snd x)) // Keeping the order of words in the sentence, though we don't do anything with it yet
        let score = (new GScores ()).Execute(words) |> Seq.sum // For now, we're just doing the sum of all scores
        (score % 10 + 75) // That's pretty basic too