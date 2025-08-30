using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TblPermission
{
    [Key]
    public int IntId { get; set; }

    public string? ChrPermission { get; set; }
}
