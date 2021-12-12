namespace MeterReadings.Shared.Exceptions
{
    public class EntityNotFoundException : Exception
    {
    }
    public class EntityNotFoundException<TEntityType> : EntityNotFoundException
    {
    }
}