namespace InventorySystemWebUI.ViewModel;

public class AssignRoleVM
{
    public string UserName { get; set; }
    public string Role { get; set; }
}

public class UserWithRolesVM
{
    public string UserName { get; set; }
    public List<string> Roles { get; set; }
}
