namespace NtlStudio.TreasureHunters.Model
{
    public interface IField
    {
        FieldCell this[int x, int y] { get; }
    }
}