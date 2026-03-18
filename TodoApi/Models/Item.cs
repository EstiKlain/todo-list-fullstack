using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public partial class Item
{
    public int Id { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Task name cannot be empty")]
    public string? Name { get; set; }

    public bool? IsComplete { get; set; }

    [Required]
    public int UserId { get; set; }
}
