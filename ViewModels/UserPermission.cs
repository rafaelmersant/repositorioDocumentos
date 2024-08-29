using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositorioDocumentos.ViewModels
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public string UserName { get; set; }
        public string CreatedDate { get; set; }
    }
}