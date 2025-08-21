using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class UploadFile
{
    public Guid UploadFileId { get; set; }

    public string? Name { get; set; }

    public string? Src { get; set; }

    public Guid TodoId { get; set; }
}
