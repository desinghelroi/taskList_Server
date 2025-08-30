using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TblCustomer
{
     [Key]
    public int IntId { get; set; }

    public string? ChrCustomerName { get; set; }

    public string? ChrCustomerCode { get; set; }
}
