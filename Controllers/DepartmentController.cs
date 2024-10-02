using Microsoft.AspNetCore.Mvc;
using agriWebAPI.Model.Department;
using System.Data.SqlClient;
using Dapper;

namespace agriWebAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {


        private readonly string _connectionString;

        public DepartmentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
       
        [HttpPost]
        public async Task<ActionResult> insupdDepartment([FromBody] department department)
        {
            int returnCode = 0;
            string returnMsg = string.Empty;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                

                DynamicParameters dynamic = new DynamicParameters();

                dynamic.Add("@department_code", department.departmentCode, System.Data.DbType.String);
                dynamic.Add("@department_name", department.departmentName, System.Data.DbType.String);
                dynamic.Add("@department_address", department.departmentAddress, System.Data.DbType.String);

                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("insupd_department", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 600;

                    command.Parameters.AddRange(dynamic.ParameterNames.Select(name => new SqlParameter(name, dynamic.Get<object>(name))).ToArray());

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {

                            returnCode = (int)reader["@retval"];
                            returnMsg = (string)reader["@msg"];

                        }
                    }

                }
            }


            if (returnCode < 0)
            {
                return BadRequest(returnMsg);
            }

            return Ok(returnMsg);

        }

        [HttpGet]
        public async Task<IActionResult> getDepartment()
        {
            var departments = new List<department>();

            using (SqlConnection connect = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("get_department", connect);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await connect.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        departments.Add(new department
                        {
                            departmentCode = reader["department_code"].ToString(),
                            departmentName = reader["department_name"].ToString(),
                            departmentAddress = reader["department_address"].ToString()
                        }); 
                    }
                }

                
            }

            return Ok(departments);
        }

        [HttpDelete("{departmentCode}")]

        public async Task<IActionResult> deleteDepartment(string departmentCode)
        {
            int returnCode = 0;
            string returnMsg = string.Empty;

            if (string.IsNullOrEmpty(departmentCode))
            {
                return BadRequest("Employee code is required.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                DynamicParameters dynamic = new DynamicParameters();

                dynamic.Add("@department_code", departmentCode, System.Data.DbType.String);

                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("delete_department", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 600;

                    command.Parameters.AddRange(dynamic.ParameterNames.Select(name => new SqlParameter(name, dynamic.Get<object>(name))).ToArray());

                    //await command.ExecuteNonQueryAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {

                            returnCode = (int)reader["@retval"];
                            returnMsg = (string)reader["@msg"];

                        }
                    }                   
                }
            }

            if (returnCode < 0)
            {
                return BadRequest(returnMsg);
            }

            return Ok(returnMsg);
        }

    }
}
