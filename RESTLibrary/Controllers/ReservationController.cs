using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NJsonSchema.Annotations;
using RESTLibrary.Models;
using System;

namespace RESTLibrary.Controllers
{
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> logger;
        private readonly IReservationService reservationService;
        public ReservationController(ILogger<ReservationController> logger, IReservationService reservationService)
        {
            this.logger = logger;
            this.reservationService = reservationService;
        }

        [Authorize]
        [HttpPost("reserve/book")]
        public ActionResult<ReserveBookResponse> ReserveBook([FromBody] ReserveBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reserved = reservationService.AddReservation(request.Reservation);

            return Ok(new ReserveBookResponse {  Reserved = reserved } );
        }

        public class ReserveBookRequest
        {
            private readonly Reservation reservation = new Reservation();

            public ReserveBookRequest()
            {
                reservation.ReservationTimeUtc = DateTime.UtcNow;
            }

            [JsonSchemaIgnore]
            public Reservation Reservation 
            {
                get { return reservation; } 
            }

            public string UserEmail 
            {
                get { return reservation.UserEmail; }
                set { reservation.UserEmail = value; } 
            }
            public string BookId 
            {
                get { return reservation.BookId; } 
                set { reservation.BookId = value; } 
            }
        }

        public class ReserveBookResponse
        {            
            public bool Reserved { get; set; }
        }
    }
}