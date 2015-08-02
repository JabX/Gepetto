-- Computes the gScore for all words in the Word table.
-- All words that share a synonym relation have the same gScore
-- Synonyms are stored in the Synonym table
create procedure computeGScore
as
	-- Temp table to store the scores
	create table Score (wordId int, gScore int, gScore2 int)
	-- Initialized with all synonym relations, regardless of duplicates
	insert into Score select wordId1, synonymId, 0 from Synonym
	insert into Score select wordId2, synonymId, 0 from Synonym

	-- We will iterate through all possible scores from the temp table Score
	-- Since we will delete a crap ton of scores during the procedure, we're getting the min value at each loop, instead of iterating through the table
	declare @gScore int = (select min(gScore) from Score)
	declare @keepGoing bit = 1

	-- @keepGoing equals 0 when there isn't any score left to compute
	while @keepGoing = 1
	begin
		print 'score: ' + cast(@gScore as varchar)
		-- To start the setScore recursion, we'll tell it to replace the current score by the current score
		declare @vGScore varchar(max) = (select convert(varchar, @gScore))
		exec setScore @gScore, @vGScore 
		print 'score done: ' + cast(@gScore as varchar)
		
		-- Next score is min value, but greater than the latest one
		set @gScore = (select min(gScore) from Score where gScore > @gScore)
		if @gScore is null begin set @keepGoing = 0 end
	end

	-- Renumbering the scores, to keep smaller indexes
	update ss set gScore2 = 
	(
		select score from 
		(
			select 
				row_number() over (order by gScore) as score,
				gScore 
			from (select distinct gScore from Score) as a
		) as s 
		where s.gScore = ss.gScore
	)
	from Score ss

	-- Updates Word with the computed values
	update Word set gScore = 0
	update Word set gScore = Score.gScore2 from Score where Score.wordId = Word.wordId

	-- Now that we're here, we're still missing scores for words without synonyms
	-- The plan is to iterate through all scores left at 0 and give them the next score in line

	declare Words cursor for select wordId from Word where gScore = 0 order by 1
	declare @scoreCount int, @word int

	set @scoreCount = (select max(gScore) + 1 from Word)

	open Words
	fetch next from Words into @word
	while @@fetch_status = 0
	begin
		update Word set gScore = @scoreCount where wordId = @word
		set @scoreCount = @scoreCount + 1
		fetch next from Words into @word
	end

	-- And we delete the temp table
	drop table Score
return 0