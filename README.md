# proiect-ii
proiect Inginerie Industriala<br/>


Prerequisites:
	SQL Server Express, SSMS for the database<br/>
	in the Windows Forms Application run "Install-Package Microsoft.Data.SqlClient" in the NuGet Package Manager<br/>


	from SQL Server Express get the connection string, might have to add Encrypt=False; in order to get past the: "A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)" error<br/>
	the connection string should look something like this: @"Server=localhost\SQLEXPRESS;Database=Products;Trusted_Connection=True;Encrypt=False;"
	
