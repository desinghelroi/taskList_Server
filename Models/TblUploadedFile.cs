using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TblUploadedFile
{
    [Key]
    public int IntId { get; set; }

    public int IntTaskId { get; set; }

    public string? ChrOriginalFileName { get; set; }

    public string? ChrSavedFileName { get; set; }
}
