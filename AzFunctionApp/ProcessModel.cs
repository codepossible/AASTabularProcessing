using System;
using System.Configuration;
using System.Net;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.SqlServerAnaylsisServerTabularProcessing;

namespace AzFunctionApp
{
    /// <summary>
    /// Azure function to process the specified database.
    /// </summary>
    public static class ProcessModel
    {
        /// <summary>
        /// Process the specified tabular model.
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <param name="databaseName">Name of the database to process</param>
        /// <param name="log">Instance of log writer</param>
        /// <returns>Returns the result of the processing of the model</returns>
        [FunctionName("ProcessModel")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", 
            Route = "ProcessTabularModel/{databaseName}")]HttpRequestMessage req,
                string databaseName,
                TraceWriter log)
        {
            log.Info("Received request to process the model " + databaseName);           
            try
            {
                SqlServerAnalysisServerTabular tabularModel = new SqlServerAnalysisServerTabular()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["SsasTabularConnection"].ConnectionString,
                    DatabaseName = databaseName ?? ConfigurationManager.AppSettings["DatabaseName"]
                };

                tabularModel.ProcessModelFull();                
            }
            catch (Exception e)
            {
                log.Info($"C# HTTP trigger function exception: {e.ToString()}");
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            return req.CreateResponse(HttpStatusCode.OK, "Processed model:" + databaseName);
        }
    }
}