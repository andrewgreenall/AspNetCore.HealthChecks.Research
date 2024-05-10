using System;
using System.Data.SqlClient;
using AspNetCore.Hangfire.HealthChecks.Models.Data;

namespace AspNetCore.Hangfire.HealthChecks
{
    /// <summary>
    /// Factory Design pattern for processing Hangfire data
    /// </summary>
    public class HangfireFactory
    {
        public IHangfireProcessor CreateHangfireProcessor(string processorType)
        {
            switch (processorType)
            {
                case "ServerId":
                    return new ServerId();
                case "ServerEntity":
                    return new ServerEntity();
                // Add more cases for different processor types if needed
                default:
                    throw new ArgumentException("Invalid processor type");
            }
        }
    }

    public abstract class OperationResult
    {
    }

    public class StringResult : OperationResult
    {
        public string Result { get; set; }
    }

    public class ServerIdModelResult : OperationResult
    {
        public ServerIdModel Result { get; set; }
    }

    public class ServerEntityResult : OperationResult
    {
        public Server Result { get; set; }
    }

    public interface IHangfireProcessor
    {
        OperationResult ProcessData(string data);
        OperationResult ProcessData(SqlDataReader server);
    }

    public abstract class HangfireProcessor : IHangfireProcessor
    {
        public virtual OperationResult ProcessData(string data)
        {
            throw new NotImplementedException();
        }

        public virtual OperationResult ProcessData(SqlDataReader server)
        {
            throw new NotImplementedException();
        }
    }

    public class ServerId : HangfireProcessor
    {
        public override OperationResult ProcessData(string data)
        {
            return new ServerIdModelResult { Result = HangFireUtils.ConvertServerIdToModel(data) };
        }
        public ServerId()
        {
        }
    }

    public class ServerEntity : HangfireProcessor
    {
        public override OperationResult ProcessData(SqlDataReader server)
        {
            var result = new Server
            {
                Id = server["Id"].ToString(),
                Data = server["Data"].ToString(),
                LastHeartbeat = (DateTime)server["LastHeartbeat"]
            };
            return new ServerEntityResult { Result = result };
        }
        public ServerEntity()
        {
        }
    }
}
