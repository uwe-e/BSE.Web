using System;
using System.ComponentModel.DataAnnotations;

namespace BSE.Identity.Web.Models.EntityFramework
{
    public class RefreshToken
    {
        [StringLength(500)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime IssuedTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        //[Column(TypeName = "varchar(500)")]
        public string SerializedTicket { get; set; }
    }
}