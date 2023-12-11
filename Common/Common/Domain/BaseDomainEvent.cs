using MediatR;

namespace Services.Common.Domain
{
    public class BaseDomainEvent : INotification
    {
        public DateTime CreationDate { get; protected set; }
        public BaseDomainEvent()
        {
            CreationDate = DateTime.Now;
        }
    }
}
