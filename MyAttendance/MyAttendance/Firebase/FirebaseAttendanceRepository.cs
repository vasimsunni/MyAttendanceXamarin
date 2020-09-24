using Firebase.Database;
using Firebase.Database.Query;
using MyAttendance.Firebase.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAttendance.Firebase
{
    public class FirebaseAttendanceRepository
    {
        

        private readonly string ChildName = "Attendance";

        readonly FirebaseClient firebase = new FirebaseClient("https://myattendance-b335d.firebaseio.com/");


        public async Task<List<AttendanceModel>> GetAllAttendance()
        {
            return (await firebase
                .Child(ChildName)
                .OnceAsync<AttendanceModel>()).OrderByDescending(x => x.Object.Date).Select(item => new AttendanceModel
                {
                    Id = item.Object.Id,
                    Date = item.Object.Date,
                    CheckInTime = item.Object.CheckInTime,
                    CheckoutTime = item.Object.CheckoutTime,
                    TotalHours = item.Object.TotalHours,
                    Comment = item.Object.Comment
                }).ToList();
        }

        public async Task AddAttendance(AttendanceModel attendance)
        {
            var jsonAttendance = JsonConvert.SerializeObject(attendance);

            await firebase
                .Child(ChildName)
                .PostAsync(jsonAttendance);
        }

        public async Task<AttendanceModel> GetAttendance(Guid id)
        {
            return (await GetAllAttendance()).Where(x => x.Id == id).LastOrDefault();
        }
        public async Task<List<AttendanceModel>> GetAttendance(DateTime date)
        {

            firebase.Child(ChildName).Shallow();
            return (await GetAllAttendance()).Where(x => x.Date.Value.Date == date.Date).OrderBy(x => x.CheckInTime).ToList();
        }
        public async Task UpdateAttendance(AttendanceModel attendance)
        {
            var jsonAttendance = JsonConvert.SerializeObject(attendance);

            var toUpdateAttendance = (await firebase
                .Child(ChildName)
                .OnceAsync<AttendanceModel>()).FirstOrDefault(a => a.Object.Id == attendance.Id);

            await firebase
                .Child(ChildName)
                .Child(toUpdateAttendance.Key)
                .PutAsync(jsonAttendance);
        }

        public async Task DeleteAttendance(Guid id)
        {
            var toDeleteAttendance = (await firebase
                 .Child(ChildName)
                 .OnceAsync<AttendanceModel>()).FirstOrDefault(a => a.Object.Id == id);

            await firebase.Child(ChildName).Child(toDeleteAttendance.Key).DeleteAsync();

        }
    }
}
