using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class Status
{
    [Key]
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;
}
