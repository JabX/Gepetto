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

    // Possible error types
    type ErrorType =
        | Unknown
        | QuestionType
        | Sentence

    // Words that validates the sentence as a Percentage question
    let percentWords = 
        Set.ofList [
            "pourcentage"
            "proportion"
            "ratio"
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
        | Error of ErrorType

    // The path of the binaries with the pos models isn't always the same
    let posModel = match System.AppDomain.CurrentDomain.RelativeSearchPath with
                   | null -> @"pos-model\" // Local context
                   | path -> path + @"\pos-model\" // Web context

    let analyzer = new AggregateAnalyzer ()
    posModel + "fr-sent.bin" |> analyzer.Add
    posModel + "fr-token.bin" |> analyzer.Add
    posModel + "fr-pos-maxent.bin" |> analyzer.Add

    type WordType = SqlEnumProvider<"select typeId from WordType", Config.DbString>
    type GScores = SqlCommandProvider<"select gScore from getScores(@data)", Config.DbString>
    type InputWord = GScores.InputWord

    // Active pattern to recognize the questions
    let (|IsPercentage|IsAmount|IsNothing|) (sentence: string) =
        let words = (sentence.ToLower ()).Split ' '
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
            Error QuestionType
        else
            // We're using a POS-Tagger to tag the sentence
            let doc = new Document ("fr", sentence)
            analyzer.Analyze doc

            // Checking if the sentence has been correctly identified
            if (doc.Sentences |> Seq.head).TagProbability < 0.9
                then Error Sentence
            else
                // Tags are in the Tokens of the first Sentence of the Document
                let words = (doc.Sentences |> Seq.head).Tokens
                            |> Seq.filter (fun token -> (token.POSTagProbability > 0.85 || token.POSTag = "V") // Removes all crap tokens, but verbs are always good
                                                        && token.Lexeme.Length > 3  // Short words
                                                        && not (Set.union amountWords percentWords |> Set.contains token.Lexeme)) // And key words
                            |> Seq.map (fun token ->
                                // Matching the tags with the types from the DB
                                let dbToken = match token.POSTag with
                                              | "A" -> WordType.adj
                                              | "V" -> WordType.vrb
                                              | "N" -> WordType.nom
                                              | "ADV" -> WordType.adv
                                              | _ -> WordType.itj // We're not using all word types, and we'll use this one as the trash one
                                (token.Lexeme, dbToken))
                            |> Seq.filter (fun x -> snd x <> WordType.itj)
                            |> Seq.map (fun x -> new InputWord (0, fst x, snd x))

                let score = (new GScores ()).Execute words |> Seq.sum // For now, we're just doing the sum of all scores

                // Final output
                match sentence with
                | IsPercentage -> Percentage (score % 10 + 75) // Basic stuff
                | IsAmount -> Amount <| enum<HowMany> (score % 5) // Here too
                | _ -> Error Unknown // Keeping the compiler happy (can't happen)