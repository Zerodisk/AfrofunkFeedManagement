Sub DBAdd 

	Const adOpenStatic = 3
	Const adLockOptimistic = 3
	Const adUseClient = 3
	
	Dim conn, sql, rs
	Set conn = CreateObject("ADODB.Connection")
	conn.Open "Provider=SQLOLEDB.1;Data Source=TanNovo\SQLEXPRESS;Initial Catalog=AfrofunkOnline;User Id=afrofunk;Password=manutd01;"
	
	sql = "Insert into FeedBatch(MID) Values (100)"
	conn.Execute(sql)
	Msgbox("new batch is inserted")
	
	sql = "select count(*) from FeedBatch where (DateDownloaded is null or DateProcessed is null or DatePushed is null)"
	Set rs = conn.Execute(sql)
	
	If rs(0) = 1 then
		Msgbox("Ready to run an app")
	Else
		Msgbox("Check FeedBatch table now, there are " & rs(0) & " batches pending !!!!")
	End If
	
	rs.Close
	Set rs = Nothing
	conn.Close
	Set conn = Nothing
	
	
End Sub


Call DBAdd