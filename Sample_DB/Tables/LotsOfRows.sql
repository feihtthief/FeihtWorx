use DataWorkerTest
go

-- drop table [dbo].[LotsOfRows] 

create table [dbo].[LotsOfRows] (
	 [ID]            int          not null IDENTITY(1,1) 
	,[Number]        int          not null
	,[Name]          varchar(255) not null
	,CONSTRAINT [PK_LotsOfRows] PRIMARY KEY CLUSTERED ( [ID] )
)
go

set nocount on
declare @cnt int = 0
declare @total int = 1 * 1000 * 1000
begin transaction
	while @cnt < @total
	begin
		set @cnt = @cnt + 1
		insert into [dbo].[LotsOfRows]
		 ( [Number] , [Name]                                             ) values
		 ( @cnt     , right('00000000000'+cast(@cnt as varchar(255)),11) )
	end
commit transaction

select count(*) from LotsOfRows
