namespace Gepetto

open FSharp.Data

module Dal =
    
    type Word = {
        id: int
        name: string
        ``type``: string
    }

    type WordByName = SqlCommandProvider<"
        select wordId, wordName, typeName 
        from Word 
        join WordType on Word.typeId = WordType.typeId
        where wordName like @search + '%'
        order by wordName", Config.DbString>

    let SearchWordByName word = 
        (new WordByName()).Execute word
        |> Seq.map (fun x -> { id = x.wordId; name = x.wordName; ``type`` = x.typeName })

    type WordById = SqlCommandProvider<"select wordName from Word where wordId = @id", Config.DbString>

    let GetWord wordId =
        (new WordById()).Execute wordId |> Seq.head

    type FormsForWord = SqlCommandProvider<"select formName from Form where wordId = @id", Config.DbString>

    let GetForms wordId =
        (new FormsForWord()).Execute wordId

    type SynonymsForWord = SqlCommandProvider<"
        select wordName 
        from Synonym
        join Word on Word.wordId = Synonym.wordId1
        where wordId2 = @id1
        union
        select wordName 
        from Synonym
        join Word on Word.wordId = Synonym.wordId2
        where wordId1 = @id2", Config.DbString>

    let GetSynonyms wordId =
        (new SynonymsForWord()).Execute (wordId, wordId)