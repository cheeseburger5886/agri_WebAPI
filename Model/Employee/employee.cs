namespace agriWebAPI.Model.Employee
{
    public class employee
    {
        public string departmentCode { get; set; }
        public string employeeCode { get; set; }
        public string empFirstName { get; set; }
        public string empLastName { get; set; }
        public string empGender { get; set; }
        public DateTime empBirthDate { get; set; }
        public DateTime empDateJoined { get; set; }
        public string empAddress { get; set; }
        public byte[] empPhoto { get; set; }

    }
}
