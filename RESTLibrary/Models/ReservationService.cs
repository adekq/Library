using System;
using System.Collections.Generic;

namespace RESTLibrary.Models
{
    public class Reservation
    {
        public string UserEmail { get; set; }
        public string BookId { get; set; }
        public DateTime ReservationTimeUtc { get; set; }
    }

    public class BookQueue
    {
        public string BookId { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    public interface IReservationService
    {
        public bool AddReservation(Reservation reservation);
    }

    public interface IReservationServicePersister
    {
        public bool StoreReservation(Reservation reservation);        
        public BookQueue ReadBookQueue(string bookId);        
    }

    public class ReservationService : IReservationService
    {
        private readonly IReservationServicePersister persister;

        public ReservationService(IReservationServicePersister persister)
        {
            this.persister = persister;
        }

        public bool AddReservation(Reservation reservation)
        {
            return persister.StoreReservation(reservation);
        }
    }
}