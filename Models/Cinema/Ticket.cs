﻿using Cinema_Management_System.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Management_System.Models.Cinema
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int ScreeningId { get; set; }

        [Required]
        public required int SeatId { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required DateTime PurchaseDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal FinalPrice { get; set; }

        [ForeignKey("ScreeningId")]
        public Screening? Screening { get; set; }

        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
