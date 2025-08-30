using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TblAdmin
{
    [Key]
    public int IntId { get; set; }

    public string? ChrFirstName { get; set; }

    public string? ChrLastName { get; set; }

    public string? ChrUserName { get; set; }

    public string? ChrPassword { get; set; }

    public DateTime? DtLastLogin { get; set; }
}
