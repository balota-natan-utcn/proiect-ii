# proiect-ii
proiect Inginerie Industriala  


Prerequisites:  
	SQL Server Express  
 	SSMS for the database  
	in the Windows Forms Application run in NuGet Package Manager Console:	"Install-Package Microsoft.Data.SqlClient"
 										"Install-Package iTextSharp"


from SQL Server Express get the connection string, might have to add Encrypt=False; in order to get past the: "A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)" error  
	the connection string should look something like this: @"Server=localhost\SQLEXPRESS;Database=DBProiectII;Trusted_Connection=True;Encrypt=False;"
	
