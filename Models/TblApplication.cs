using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TblApplication
{
    [Key]
    public int IntId { get; set; }

    public int? IntCustomerId { get; set; }

    public string? ChrApplicationName { get; set; }
}
