﻿namespace WebUI.DTOs
{
    public class RolesAndUsersViewModel
    {
       public IEnumerable<string> Users { get; set; }
       public IEnumerable<RoleViewModel> Roles { get; set; }
    }
}
