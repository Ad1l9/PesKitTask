﻿namespace PesKitTask.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InstaLink { get; set; }
        public string FbLink { get; set; }
        public string TwitterLink { get; set; }
        public string LinkeednLink { get; set; }
		public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
    }
}
