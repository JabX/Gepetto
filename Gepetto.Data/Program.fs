namespace Gepetto.Data

open FSharp.Data
open FSharp.Data.SqlClient
open System.Data.SqlClient

module Main =
    type Data = JsonProvider<"data/template.json">

    let wordsToFix = [
        "appuyer"
        "attacher"
        "avoyer"
        "baiser"
        "baisser"
        "bitter"
        "boucher"
        "boulanger"
        "boxer"
        "bénéficier"
        "bûcher"
        "canter"
        "carter"
        "challenger"
        "chopper"
        "clocher"
        "cocher"
        "conseiller"
        "contester"
        "corner"
        "coucher"
        "doubler"
        "driver"
        "débotter"
        "débucher"
        "déjeuner"
        "dîner"
        "ester"
        "exister"
        "fader"
        "flipper"
        "fromager"
        "gabarier"
        "gambier"
        "grimper"
        "herbager"
        "interviewer"
        "jogger"
        "jumper"
        "lainer"
        "lancer"
        "lever"
        "luger"
        "lâcher"
        "manager"
        "mixer"
        "noyer"
        "officier"
        "oranger"
        "pailler"
        "palmer"
        "parler"
        "placer"
        "plancher"
        "pointer"
        "porter"
        "poster"
        "putter"
        "pêcher"
        "quarter"
        "radier"
        "ranger"
        "reporter"
        "rewriter"
        "rocher"
        "rucher"
        "réchauffer"
        "scanner"
        "skipper"
        "sonnailler"
        "souper"
        "sprinter"
        "squatter"
        "supporter"
        "surfer"
        "toaster"
        "toucher"
        "tuner"
        "écailler"
    ]

    let firstGroup = [
        "a"
        "ai"
        "aient"
        "ais"
        "ait"
        "ant"
        "as"
        "asse"
        "assent"
        "asses"
        "assiez"
        "assions"
        "e"
        "ent"
        "era"
        "erai"
        "eraient"
        "erais"
        "erait"
        "eras"
        "erez"
        "eriez"
        "erions"
        "erons"
        "eront"
        "es"
        "ez"
        "iez"
        "ions"
        "âmes"
        "ât"
        "âtes"
        "ons"
        "èrent"
        "és"
        "é"
        "ée"
        "ées"
    ]

    [<EntryPoint>]
    let main _ =
        let data = Data.Load "../../data/data.json"
        
        let fixedData = data.Words |> Array.map (fun word ->
            let wd = word.Word 
            if word.Type = "verbe" && List.contains wd wordsToFix  then
                let forms = firstGroup |> List.map (fun x ->
                    if wd.[wd.Length - 2] = 'y' then
                        wd.Substring(0, wd.Length - 2) + x
                    else if wd.[wd.Length - 2] = 'g' then
                        wd.Substring(0, word.Word.Length - 1) + "a" + x
                    else
                        wd.Substring(0, word.Word.Length - 1) + x) 
                                       |> List.toArray
                new Data.Word (word.Word, word.Type, forms, word.Synonyms)
            else word
        )


    (*
    type SelectTypeWord = SqlCommandProvider<"select typeId, typeName from WordType", Config.DbString>

    type GepettoDb = SqlProgrammabilityProvider<Config.DbString>

    [<EntryPoint>]
    let main _ =
        use selectTypeWord = new SelectTypeWord ()
        let typeWord = selectTypeWord.Execute ()
                       |> Seq.cast<SelectTypeWord.Record>
                       |> Seq.map (fun x -> (x.typeName, x.typeId))
                       |> Map

        let data = Data.Load "../../data/data.json"
        let wordTable = new GepettoDb.dbo.Tables.Word ()
        let formTable = new GepettoDb.dbo.Tables.Form ()
        let synonymTable = new GepettoDb.dbo.Tables.Synonym ()

        let mutable id = 1

        for item in data.Words do
            wordTable.AddRow (id, item.Word, typeWord.[item.Type])
            for form in item.Forms do
                formTable.AddRow (id, form)
            for synonym in item.Synonyms do
                if (synonym.Split ' ').Length = 1 then
                    synonymTable.AddRow (id, synonym)
           
            id <- id + 1
            if id % 2000 = 0 then
                wordTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
                formTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
                synonymTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
                wordTable.Rows.Clear ()
                formTable.Rows.Clear ()
                synonymTable.Rows.Clear ()
                printfn "%i" id

        wordTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
        formTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
        synonymTable.BulkCopy(copyOptions = SqlBulkCopyOptions.TableLock)
        printfn "%i" id
        *)
        System.Console.ReadKey () |> ignore
        0
        