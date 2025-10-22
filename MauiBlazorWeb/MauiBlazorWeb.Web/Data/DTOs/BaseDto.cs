using System;

namespace MauiBlazorWeb.Web.Data.DTOs
{
    /// <summary>
    /// Base DTO for transferring entity data between client and server
    /// </summary>
    public abstract class BaseDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}