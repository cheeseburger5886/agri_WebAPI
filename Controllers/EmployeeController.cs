using Microsoft.AspNetCore.Mvc;
using agriWebAPI.Model.Employee;
using System.Data.SqlClient;
using Dapper;

namespace agriWebAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly AppDbContext _appDbContext;

        public EmployeeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<ActionResult> insupdDepartment([FromBody] employee employee)
        {

            int returnCode = 0;
            string returnMsg = string.Empty;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                DynamicParameters dynamic = new DynamicParameters();

                dynamic.Add("@department_code", employee.departmentCode, System.Data.DbType.String);
                dynamic.Add("@employee_code", employee.employeeCode, System.Data.DbType.String);
                dynamic.Add("@firstname", employee.empFirstName, System.Data.DbType.String);
                dynamic.Add("@lastname", employee.empLastName, System.Data.DbType.String);
                dynamic.Add("@gender", employee.empGender, System.Data.DbType.String);
                dynamic.Add("@birth_date", employee.empBirthDate, System.Data.DbType.DateTime);
                dynamic.Add("@join_dated", employee.empDateJoined, System.Data.DbType.DateTime);

                if (employee.empPhoto != null && employee.empPhoto.Length > 0)
                {
                    dynamic.Add("@photo", employee.empPhoto, System.Data.DbType.Byte);
                }

                dynamic.Add("@photo", employee.empPhoto, System.Data.DbType.Byte);
                dynamic.Add("@address", employee.empAddress, System.Data.DbType.String);

                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("insupd_employee", connection))
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
        public async Task<IActionResult> getEmployee()
        {
            var employees = new List<employee>();

            using (SqlConnection connect = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("get_employee", connect);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await connect.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new employee
                        {
                            departmentCode = reader["department_code"].ToString(),
                            employeeCode = reader["employee_code"].ToString(),
                            empFirstName = reader["first_name"].ToString(),
                            empLastName = reader["last_name"].ToString(),
                            empGender = reader["gender"].ToString(),
                            empAddress = reader["address"].ToString(),
                            empBirthDate = Convert.ToDateTime(reader["birth_date"]),
                            empDateJoined = Convert.ToDateTime(reader["date_joined"])
                            //empPhoto = reader["photo"] as byte[]
                        });
                    }
                }


            }

            return Ok(employees);
        }

        [HttpGet("{employeeCode}")]
        public async Task<IActionResult> getEmployeeByID(string employeeCode)
        {
            var employees = new List<employee>();

            using (SqlConnection connect = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("get_employee_by_id", connect);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@employeeCode", employeeCode);

                await connect.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new employee
                        {
                            departmentCode = reader["department_code"].ToString(),
                            employeeCode = reader["employee_code"].ToString(),
                            empFirstName = reader["first_name"].ToString(),
                            empLastName = reader["last_name"].ToString(),
                            empGender = reader["gender"].ToString(),
                            empAddress = reader["address"].ToString(),
                            empBirthDate = Convert.ToDateTime(reader["birth_date"]),
                            empDateJoined = Convert.ToDateTime(reader["date_joined"]),
                            empPhoto = reader["photo"] as byte[]
                        });
                    }
                }


            }

            return Ok(employees);
        }


        [HttpDelete("{employeeCode}")]
        public async Task<IActionResult> deleteEmployee(string employeeCode)
        {
            int returnCode = 0;
            string returnMsg = string.Empty;

            if (string.IsNullOrEmpty(employeeCode))
            {
                return BadRequest("Employee code is required.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                DynamicParameters dynamic = new DynamicParameters();

                dynamic.Add("@employee_code", employeeCode, System.Data.DbType.String);

                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("delete_employee", connection))
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
