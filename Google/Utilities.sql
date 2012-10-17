-- Enable CLR functionality in an existing SQL Server database
alter database <database> set trustworthy on;
exec sp_configure 'clr_enabled', 1;
reconfigure;

-- Drop all of the CLR dependencies for the Google Analytics API functionality
DROP ASSEMBLY [Skyscanner.Google.Analytics];
DROP ASSEMBLY [Google.GData.Analytics];
DROP ASSEMBLY [Google.GData.Client];

-- Deploy the CLR dependencies
CREATE ASSEMBLY [Google.GData.Client]
AUTHORIZATION [dbo]
FROM '<path>\Google.GData.Client.dll'
WITH PERMISSION_SET = UNSAFE;

CREATE ASSEMBLY [Google.GData.Analytics]
AUTHORIZATION [dbo]
FROM '<path>\Google.GData.Analytics.dll'
WITH PERMISSION_SET = UNSAFE;

CREATE ASSEMBLY [Skyscanner.Google.Analytics]
AUTHORIZATION [dbo]
FROM '<path>\Skyscanner.Google.Analytics.dll'
WITH PERMISSION_SET = UNSAFE;
GO

-- Create a stored procedure to utilize the Google Analytics API functionality
CREATE PROCEDURE uspGA_Query
(
    @ProfileIDs nvarchar(255),
    @Username nvarchar(64),
    @Password nvarchar(16),
    @DateFrom datetime,
    @DateTo datetime,
    @Dimensions nvarchar(255),
    @Metrics nvarchar(255),
    @Sort nvarchar(255) = '',
    @Segments nvarchar(255) = '',
    @Filters nvarchar(255) = ''
)
AS EXTERNAL NAME [Skyscanner.Google.Analytics].[Skyscanner.Google.Analytics].[Query];
GO

-- Perform a test query
--
-- For a full set of dimensions and metrics which can be used during queries, see:
-- http://code.google.com/apis/analytics/docs/gdata/dimsmets/dimsmets.html
--
declare @data table (date datetime, visits int)

exec uspGA_Query
	@ProfileIDs = 'ga:<profileid>',
	@Username = '<google-account>',
	@Password = '<google-password>',
	@DateFrom = '<YYYY-MM-DD>',
	@DateTo = '<YYYY-MM-DD>',
	@Dimensions = 'ga:<dimension>',
	@Metrics = 'ga:<metric>,ga:<metric>',
	@Sort = 'ga:<dimension|metric>,-ga:<dimension|metric>';