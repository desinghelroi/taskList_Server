using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TasksOld
{
    [Key]
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public DateTime LastChangeDate { get; set; }

    public int StatusId { get; set; }

    public int PriorityId { get; set; }

    public string ApplicationName { get; set; } = null!;

    public string DelegatedTo { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Visible { get; set; }

    public bool SeriousBug { get; set; }

    public bool SmallBug { get; set; }
}
