using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Google.GData.Analytics;

using Microsoft.SqlServer.Server;

namespace Skyscanner.Google
{
    public class Analytics
    {
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void Query(SqlString profileids, SqlString username, SqlString password, SqlDateTime dateFrom, 
                                 SqlDateTime dateTo, SqlString dimensions, SqlString metrics, SqlString sort, 
                                 SqlString segments, SqlString filters)
        {
            // Google Analytics service endpoint
            const string dataFeedUrl = "https://www.google.com/analytics/feeds/data";

            // Create a GData.net service object to contact the endpoint with our authentication details
            AnalyticsService service = new AnalyticsService("Skyscanner Analytics");
            service.setUserCredentials(username.ToString(), password.ToString());

            // Construct and populate an analytics query object
            DataQuery query = new DataQuery(dataFeedUrl);
            query.Ids = profileids.ToString();
            query.Metrics = metrics.ToString();
            query.Dimensions = dimensions.ToString();
            query.GAStartDate = dateFrom.Value.ToString("yyyy-MM-dd");
            query.GAEndDate = dateTo.Value.ToString("yyyy-MM-dd");
            query.Sort = sort.ToString();
            query.Segment = segments.ToString();
            query.Filters = filters.ToString();
            query.NumberToRetrieve = 10000;

            // Count the number of metrics and dimensions to be returned
            int metricCount = query.Metrics.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int dimensionCount = query.Dimensions.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Length;

            // Not possible to query the API without at least one metric, so return immediately if none were specified
            if (metricCount == 0) return;

            // Gather the results from the Google Analytics API
            DataFeed dataFeed = service.Query(query);

            // Prepare a set of columns for our SQL result set
            SqlMetaData[] columns = new SqlMetaData[metricCount + dimensionCount];

            // Iterate through each of the dimensions, and begin populating the column array
            DataEntry header = (DataEntry)dataFeed.Entries[0];
            for (int i = 0; i < dimensionCount; i++)
            {
                SqlParameter col = new SqlParameter(header.Dimensions[i].Name, header.Dimensions[i].Value);
                columns[i] = new SqlMetaData
                (
                    col.ParameterName,
                    col.SqlDbType,
                    SqlMetaData.Max
                );
            }

            // Continue populating the column array with each of the metrics
            for (int i = 0; i < metricCount; i++)
            {
                SqlParameter col = new SqlParameter(header.Metrics[i].Name, header.Metrics[i].Value);
                columns[dimensionCount + i] = new SqlMetaData
                (
                    col.ParameterName,
                    col.SqlDbType,
                    SqlMetaData.Max
                );
            }

            // Create a placeholder record based on the column metadata
            SqlDataRecord record = new SqlDataRecord(columns);

            // Set up a pipe to return results to the stored procedure callee
            SqlPipe pipe = SqlContext.Pipe;
            pipe.SendResultsStart(record);

            // Iterate through the data feed results
            foreach (DataEntry entry in dataFeed.Entries)
            {
                // Populate each dimension entry in the row
                for (int i = 0; i < dimensionCount; i++)
                {
                    record.SetValue(i, entry.Dimensions[i].Value);
                }
                // Populate each metric entry in the row
                for (int i = 0; i < metricCount; i++)
                {
                    record.SetValue(dimensionCount + i, entry.Metrics[i].Value);
                }
                
                // Send the result back to the callee
                pipe.SendResultsRow(record);
            }

            // Indicate that the result set is finished
            pipe.SendResultsEnd();
        }
    }
}