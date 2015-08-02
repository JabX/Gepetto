-- Recursively set a new score value to all score values in @oldValues
-- @oldValues is a comma-delimited string

create procedure setScore
	@newScore int,
	@oldScores varchar(max)
as
	-- @oldScores being empty is the exit clause
	if @oldScores <> ''
	begin
		print 'entering setScore with oldScores: ' + @oldScores
		-- Updates the score
		update Score set gScore = @newScore where gScore in (select id from Split(@oldScores))

		-- Deletes duplicates
		;with ToDelete as (select wordId, gScore, rn = row_number() over (partition by wordId, gScore order by wordId) from Score)
		delete from ToDelete where rn > 1

		-- Gets the new scores to update, in the string form
		declare @newOldScores varchar(max) = coalesce
		((
			select stuff (
			(
				select cast(gScore as varchar) + ',' 
				from
				(
					-- They are all the scores <> @newScore attached to an id that also has a @newScore entry
					select distinct gScore from Score 
					where wordId in (select wordId from Score where gScore = @newScore) 
						and gScore <> @newScore
				) as l 
				for xml path('')
			), 1, 0, '')),
		'')
		-- Go go recursion
		exec setScore @newScore, @newOldScores
	end
return 0