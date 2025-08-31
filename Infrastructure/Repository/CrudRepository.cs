using Infrastructure.DbContext;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;
using Domain.Entities.CRUDEntities;



namespace Infrastructure.Repository
{
    public class CrudRepository :ICrudRepository
    {
        private readonly OracleDbConnection _dbConnection;
        public CrudRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("OracleConnection");
            _dbConnection = new OracleDbConnection(connectionString);
        }


        public (int status, string[] message) StudentDataEntry(StudentData studentData)
        {
            using (var connection = _dbConnection.GetConnection())
            {

                int status = 0; string[] message = new string[2];
                OracleParameter[] Params = new OracleParameter[46];
                Params[0] = _dbConnection.MakeOutParameter(OracleDbType.Int16, ParameterDirection.Output);
                Params[1] = _dbConnection.MakeOutParameter(OracleDbType.Char, ParameterDirection.Output);
              //  Params[2] = _dbConnection.MakeInParameter(studentData.ROW_STATUS, OracleDbType.Decimal);
                //Params[3] = _dbConnection.MakeInParameter(studentData.ORG_CODE, OracleDbType.Varchar2);
       
                var Status = _dbConnection.RunProcedureWithReturnValAndStatus("DPG_EMS_PREV_STUDENT_ADMISSION.DPD_PREV_STUDENT_MST_INFO", Params);
                if (Status.status == 1)
                {
                    message[0] = Status.response;
                    message[1] = "#5cb85c";
                }
                else if (Status.status == 5)
                {
                    message[0] = Status.response;
                    message[1] = "#5cb85c";
                }
                else
                {
                    message[0] = "Failed to student admission  due to " + Status.response;
                    message[1] = "#5cb85c";
                }
                return (Status.status, message);
            }
        }
    }
}
