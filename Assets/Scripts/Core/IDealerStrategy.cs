public interface IDealerStrategy
{
    /// <summary>
    /// Возвращает true если дилер должен взять ещё карту.
    /// </summary>
    bool ShouldHit(int dealerScore);
}
