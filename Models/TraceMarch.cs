using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskList_Server.Models;

public partial class TraceMarch
{
    [Key]
    public int RowNumber { get; set; }

    public int? EventClass { get; set; }

    public string? TextData { get; set; }

    public string? ApplicationName { get; set; }

    public string? NtuserName { get; set; }

    public string? LoginName { get; set; }

    public int? Cpu { get; set; }

    public long? Reads { get; set; }

    public long? Writes { get; set; }

    public long? Duration { get; set; }

    public int? ClientProcessId { get; set; }

    public int? Spid { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public byte[]? BinaryData { get; set; }

    public int? DatabaseId { get; set; }

    public string? DatabaseName { get; set; }

    public int? Error { get; set; }

    public long? EventSequence { get; set; }

    public int? GroupId { get; set; }

    public string? HostName { get; set; }

    public int? IntegerData { get; set; }

    public int? IsSystem { get; set; }

    public byte[]? LoginSid { get; set; }

    public string? NtdomainName { get; set; }

    public string? ObjectName { get; set; }

    public int? RequestId { get; set; }

    public long? RowCounts { get; set; }

    public string? ServerName { get; set; }

    public string? SessionLoginName { get; set; }

    public long? TransactionId { get; set; }

    public long? XactSequence { get; set; }
}
