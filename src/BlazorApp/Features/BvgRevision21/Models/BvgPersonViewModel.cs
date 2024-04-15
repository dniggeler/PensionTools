using System;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Features.BvgRevision21.Models;

public class BvgPersonViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(25, ErrorMessage = "Name ist zu lang.", ErrorMessageResourceName = "error.person.name")]
    public string Name { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Gender Gender { get; set; }
}
