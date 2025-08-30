using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? PassWord { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? Maccess { get; set; }

    public string? Email { get; set; }

    public int? IntCustomerId { get; set; }

    public bool? BitShowUser { get; set; }
}
