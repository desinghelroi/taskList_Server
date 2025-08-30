using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class Priority
{
    [Key]
    public int PriorityId { get; set; }

    public string Name { get; set; } = null!;
}
