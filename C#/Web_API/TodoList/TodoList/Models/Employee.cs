using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class Employee
{
    public Guid EmployeeId { get; set; }

    public string? Name { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public Guid JobTitleId { get; set; }

    public Guid DivisionId { get; set; }
}
