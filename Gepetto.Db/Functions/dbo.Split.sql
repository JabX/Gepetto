-- Splits a string on commas into a table of int
create function Split
(
    @String varchar(max)
)
returns @SplittedValues table
(
    id int primary key
)
as
begin
    declare @SplitLength int, @Delimiter varchar(5)

    set @Delimiter = ','

    while len(@String) > 0
    begin 
        select @SplitLength = (case charindex(@Delimiter,@String) when 0 then
            len(@String) else charindex(@Delimiter,@String) -1 end)

        insert into @SplittedValues
        select convert(int, substring(@String,1,@SplitLength))

        select @String = (case (len(@String) - @SplitLength) when 0 then ''
            else right(@String, len(@String) - @SplitLength - 1) end)
    end 
return  
end