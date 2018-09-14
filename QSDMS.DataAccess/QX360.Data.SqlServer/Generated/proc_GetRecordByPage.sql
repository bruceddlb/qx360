USE [RCHL_DB_]
GO

/****** Object:  StoredProcedure [dbo].[proc_GetRecordByPage]    Script Date: 07/24/2018 13:52:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



------------------------------------
--author:bruced
--time:2011-05-07
--descript：分页存储过程(效率很高)
------------------------------------

CREATE PROCEDURE [dbo].[proc_GetRecordByPage]
    @pSQL      varchar(8000),          -- 数据源（select * FROM tb）
    @keyField      varchar(255),      -- 主键字段名
    @orderFiled varchar(20),		  -- 排序字段（注意：不需加order by关键字）
    @PageSize     int = 10,           -- 页尺寸,每页显示的条目
    @PageIndex    int = 1,            -- 页码，当前第几页
    @OrderType    bit = 1,            -- 设置排序类型, 非 0 值则降序，根据主键排序
    @strWhere     varchar(255) = '',   -- 查询条件 (注意: 不要加 where)
    @pRecNums int output,				--记录数
	@pRecPages int output				--记录页数
AS
SET NOCOUNT ON
BEGIN TRAN
declare @strSQL   varchar(8000)       -- 主语句
declare @strTmp   varchar(255)        -- 临时变量
declare @strOrder varchar(255)        -- 排序类型
declare @strCountSQL	varchar(4000) -- 返回记录总数SQL
declare @totalCount   int			  -- 总记录数		
declare @Pages int					  -- 总页数				
declare @Reccount int				  -- 设置显示其实区域

--set @Pages=0
if @OrderType != 0
begin
    set @strTmp = '<(select min'
  	set @strOrder = ' order by [' + @keyField +'] desc '
  	if @@ERROR != 0 goto Err_Proc
end
else
begin
    set @strTmp = '>(select max'
    set @strOrder = ' order by [' + @keyField +'] asc '
    if @@ERROR != 0 goto Err_Proc
end
if(@strWhere != '')
	begin
    declare  @sqls1  nvarchar(4000)  
	set   @sqls1= 'select @totalCount = count(*)  from (' + @pSQL + ')T where'  + @strWhere
	exec sp_executesql  @sqls1,N'@totalCount int out',@totalCount output
	if @@ERROR != 0 goto Err_Proc
    end
else
    begin
    --exec执行结果放入@totalCount变量中
	declare  @sqls2  nvarchar(4000)  
	set   @sqls2= 'select @totalCount = count(*)   from   ('+@pSQL+')T'  
	exec sp_executesql  @sqls2,N'@totalCount int out',@totalCount output
	if @@ERROR != 0 goto Err_Proc
	end
	--总的页数
	set @Pages = @totalCount/@PageSize
	if(@totalCount % @PageSize > 0)
		set @Pages = @Pages + 1
	if(@PageIndex<=0)
		set @PageIndex = 1
	if(@PageIndex > @Pages)
	    --传入的当前页号大于总页数的情况的意外处理
		set @PageIndex = @Pages	
    if(@totalCount<>0)		
		set @Reccount = (@PageIndex-1) * @PageSize	--设置显示其实区域
	else
		set @Reccount=0
	set @pRecNums = @totalCount		--记录数
	set @pRecPages = @Pages		--记录页数
	if @@ERROR != 0 goto Err_Proc
	
--处理sql语句
set @strSQL = 'select top ' + str(@PageSize) + ' * from ('
    + @pSQL + ')T1 where [' + @keyField + ']' + @strTmp + '(['
    + @keyField + ']) from (select top ' + str(@Reccount) + ' ['
    + @keyField + '] from (' + @pSQL + ')T2' + @strOrder + ') as tblTmp)'
    + @strOrder
    if @@ERROR != 0 goto Err_Proc
--条件不为空
if @strWhere != ''
    set @strSQL = 'select top ' + str(@PageSize) + ' * from ('
        + @pSQL + ')T1 where [' + @keyField + ']' + @strTmp + '(['
        + @keyField + ']) from (select top ' + str(@Reccount) + ' ['
        + @keyField + '] from (' + @pSQL + ')T2 where ' + @strWhere + ' '
        + @strOrder + ') as tblTmp) and ' + @strWhere + ' ' + @strOrder
	if @@ERROR != 0 goto Err_Proc
--当前页码为1
if @PageIndex = 1
begin
    set @strTmp =''
    if @strWhere != ''
        set @strTmp = ' where ' + @strWhere
    set @strSQL = 'select top ' + str(@PageSize) + ' * from ('
        + @pSQL + ')T' + @strTmp + ' ' + @strOrder
    if @@ERROR != 0 goto Err_Proc
end
--排序条件不为空
if(@orderFiled<>'')
	set @strSQL='select * from ('+@strSQL+')T order by '+@orderFiled
	if @@ERROR != 0 goto Err_Proc
--返回结果记录总数
exec (@strSQL)
if @@ERROR != 0 goto Err_Proc
OK_Proc:
	commit tran
	--执行返回总记录和页数
	--select @totalCount as totalCount,@Pages as pages
	print 'success message:'+ @strSQL
	set nocount off
	return 0
Err_Proc:
	rollback tran
	print 'error message:'+@strSQL
	set nocount off
	return -1
/*************************************************
test:
declare @pRecNums int,@pRecPages int
EXEC [proc_GetRecordByPage]
		'select * from test_1
		 union all
		 select * from test_1
		',						-- 数据源
		'keyid',				-- 主键
		'seq asc',				-- 排序语块
		15,						-- 页大小
		144570,					-- 页码
		1,						-- 排序方式1：desc；0：asc
		''						-- where条件eg: name like ''name146%''
	    @pRecNums  output,--总记录
		@pRecPages  output --页数
		print @pRecNums
		print @pRecPages

***************************************************/




GO


