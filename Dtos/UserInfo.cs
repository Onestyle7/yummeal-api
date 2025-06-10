using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yummealAPI.Dtos
{
    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}