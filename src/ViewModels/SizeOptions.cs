namespace ShoesShop.ViewModels
{
    public class SizeOption
    {
        public int WarehouseId { get; init; }
        public double SizeValue { get; init; }
        public int Quantity { get; init; }
        public string Display => $"Размер {SizeValue:0.#} (в наличии: {Quantity})";
    }
}
