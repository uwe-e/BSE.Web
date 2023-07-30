using System;
using System.ComponentModel.DataAnnotations;

namespace BSE.Identity.Web.Models.EntityFramework
{
    public class RefreshToken
    {
        [StringLength(500)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string SubjectId { get; set; }
        public DateTime CreationTime { get; set; }
        public int LifeTime { get; set; }
        [StringLength(1000)]
        public string SerializedTicket { get; set; }
    }
}