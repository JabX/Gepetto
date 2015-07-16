namespace Gepetto.Data

open FSharp.Data
open FSharp.Data.SqlClient
open System.Data.SqlClient

module Main =
    type Data = JsonProvider<"data/template.json">
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
        
        System.Console.ReadKey () |> ignore 
        0
        