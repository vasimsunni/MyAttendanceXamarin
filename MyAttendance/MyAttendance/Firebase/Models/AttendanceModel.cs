using System;
using System.Collections.Generic;
using System.Text;

namespace MyAttendance.Firebase.Models
{
    public class AttendanceModel
    {
        private Guid id;
        private DateTime? checkoutTime;
        private decimal totalHours;
        private string comment;
        private DateTime? date;
        private DateTime? checkInTime;

        public Guid Id { get => id; set => id = value; }
        public DateTime? Date { get => date; set => date = value; }
        public DateTime? CheckInTime { get => checkInTime; set => checkInTime = value; }
        public DateTime? CheckoutTime { get => checkoutTime; set => checkoutTime = value; }
        public decimal TotalHours { get => totalHours; set => totalHours = value; }
        public string Comment { get => comment; set => comment = value; }
    }
}
