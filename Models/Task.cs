using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskList_Server.Models;

public partial class Task
{
    [Key]
    public int TaskId { get; set; }

    public int? IntDisplayNo { get; set; }

    public int? UserId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public DateTime? LastChangeDate { get; set; }

    public int? StatusId { get; set; }

    public int? PriorityId { get; set; }

    public int? ApplicationId { get; set; }

    public int? DelegatedTo { get; set; }

    public string? Description { get; set; }

    public bool? Visible { get; set; }

    public bool? SeriousBug { get; set; }

    public bool? SmallBug { get; set; }

    public DateTime? StartDate { get; set; }
    public string TotalHours { get; set; }

    [ForeignKey("StatusId")]
    public virtual Status Status { get; set; }
    [ForeignKey("ApplicationId")]
    public virtual TblApplication Project { get; set; }
    [ForeignKey("UserId")]
    public virtual User Users { get; set; }
}
    