DECLARE @filedate VARCHAR(20)
SET @filedate=REPLACE(REPLACE(CONVERT(VARCHAR(20),GETDATE(),20),':','.'),' ','_')

DECLARE @file_path VARCHAR(256)
SET @file_path='c:\int\BlanksBKP\Blanks_'+@filedate+'.bak'

BACKUP DATABASE [Blanks] TO  DISK = @file_path WITH NOFORMAT, INIT,  NAME = N'Blanks', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
GO