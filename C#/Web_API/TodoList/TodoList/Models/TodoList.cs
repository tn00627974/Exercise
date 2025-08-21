using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class TodoList
{
    public Guid TodoId { get; set; }
    public string? Name { get; set; }
    public DateTime InsertTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public bool Enable { get; set; }
    public int Orders { get; set; }
    public Guid InsertEmployeeId { get; set; }
    public Guid UpdateEmployeeId { get; set; }
    public virtual Employee? InsertEmployee { get; set; }
    public virtual Employee? UpdateEmployee { get; set; }

}
