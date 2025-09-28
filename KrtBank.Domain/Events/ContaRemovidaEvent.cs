namespace KrtBank.Domain.Events;

public class ContaRemovidaEvent
{
    public Guid ContaId { get; }
    public DateTime DataRemocao { get; }

    public ContaRemovidaEvent(Guid contaId)
    {
        ContaId = contaId;
        DataRemocao = DateTime.UtcNow;
    }
}

