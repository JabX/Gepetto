-- Get the scores of the words given, in order.
-- Words are defined by their name et and type
create function GetScores (@data InputWord readonly)
returns @output table ( wRank int not null, gScore int not null)
as
begin
	-- Finds them in Word and in Form
	insert into @output
		select wRank, gScore from
		(
			select gScore, wordName, typeId from Word
			union all
			select gScore, formName, typeId from Form join Word on Word.wordId = Form.wordId
		) as w
		join @data d on d.wordName = w.wordName and d.typeId = w.typeId
		order by wRank
	return
end