namespace Gepetto

open FSharp.Data

module Dal =
    
    type Word = {
        name: string
        ``type``: string
    }

    type WordByName = SqlCommandProvider<"
        select wordName, typeName 
        from Word 
        join WordType on Word.typeId = WordType.typeId
        where wordName like @search + '%'", Config.DbString>

    let SearchWordByName word = 
        (new WordByName()).Execute word
        |> Seq.map (fun x -> { name = x.wordName; ``type`` = x.typeName })