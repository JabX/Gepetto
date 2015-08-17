namespace Gepetto

open FSharp.Data
open SharpNL
open SharpNL.Analyzer

module Score =
    
    // Possible answers for an Amount question
    type HowMany =
        | All = 0
        | Most = 1
        | Some = 2
        | Few = 3
        | NoOne = 4

    // Words that validates the sentence as a Percentage question
    let percentWords = 
        Set.ofList [
            "pourcentage"
            "proportion"
        ]

    // Words that validates the sentence as an Amount question
    let amountWords = 
        Set.ofList [
            "combien"
            "nombre"
            "quantité"
        ]

    // Possible result types
    type Result = 
        | Percentage of int
        | Amount of HowMany
        | Error

    // The path of the binaries with the pos models isn't always the same
    let posModel = match System.AppDomain.CurrentDomain.RelativeSearchPath with
                   | null -> @"pos-model\" // Local context
                   | path -> path + @"\pos-model\" // Web context

    let analyzer = new AggregateAnalyzer ()
    posModel + "fr-sent.bin" |> analyzer.Add
    posModel + "fr-token.bin" |> analyzer.Add
    posModel + "fr-pos-maxent.bin" |> analyzer.Add

    type WordType = SqlEnumProvider<"select typeId from WordType", Config.DbString>
    type GScores = SqlCommandProvider<"select gScore from getScores(@data) order by wRank", Config.DbString>
    type InputWord = GScores.InputWord


    // Active pattern to recognize the questions
    let (|IsPercentage|IsAmount|IsNothing|) (sentence: string) =
        let words = sentence.ToLower().Split ' '
        if words |> Seq.exists (fun x -> Set.contains x percentWords)
            then IsPercentage
        else if words |> Seq.exists (fun x -> Set.contains x amountWords)
            then IsAmount
            else IsNothing


    /// <summary>Computes the score from the given sentence and returns it</summary>
    let Get (sentence: string) =
        // First, let's check if the input sentence is correct
        let isValid = match sentence with
                      | IsPercentage | IsAmount -> true
                      | _ -> false

        if not isValid then
            Error
        else
            // We're using a POS-Tagger to tag the sentence
            let doc = new Document ("fr", if sentence = "" then "lol" else sentence)
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
            
            // Final output
            match sentence with
            | IsPercentage -> Percentage(score % 10 + 75) // Basic stuff
            | IsAmount -> Amount(score % 5 |> enum<HowMany>) // Here too
            | _ -> Error // Keeping the compiler happy (can't happen)