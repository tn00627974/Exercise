using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class Division
{
    public Guid DivisionId { get; set; }

    public string? Name { get; set; }
}
