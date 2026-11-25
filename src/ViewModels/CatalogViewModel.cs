using ShoesShop.Commands;
using ShoesShop.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.ViewModels
{
    public class CatalogViewModel : ObservableObject
    {
        private readonly List<ShoeCardViewModel> _allCards = new();
        private readonly UserSession _session;
        private string _searchText = string.Empty;
        private string _selectedCategory = "Все категории";
        private string _selectedSort = "По умолчанию";
        public bool IsAuthorized => _session.IsAuthenticated;

        public CatalogViewModel(UserSession session)
        {
            _session = session;
            _allCards.AddRange(CatalogService.GetCatalog(DateTime.Now));

            Categories = _allCards
                .Select(c => c.CategoryName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            Categories.Insert(0, "Все категории");

            ApplyFilters();
        }

        public ObservableCollection<ShoeCardViewModel> Cards { get; } = new();
        public List<string> Categories { get; }
        public List<string> SortOptions { get; } = new()
    {
        "По умолчанию",
        "Цена: по возрастанию",
        "Цена: по убыванию"
    };

        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyFilters(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { if (SetProperty(ref _selectedCategory, value)) ApplyFilters(); }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set { if (SetProperty(ref _selectedSort, value)) ApplyFilters(); }
        }

        private void ApplyFilters()
        {
            IEnumerable<ShoeCardViewModel> query = _allCards;

            if (SelectedCategory != "Все категории")
                query = query.Where(c => c.CategoryName == SelectedCategory);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                query = query.Where(c =>c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (c.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            query = SelectedSort switch
            {
                "Цена: по возрастанию" => query.OrderBy(c => c.PriceWithDiscount),
                "Цена: по убыванию" => query.OrderByDescending(c => c.PriceWithDiscount),
                _ => query
            };

            Cards.Clear();
            foreach (var card in query)
                Cards.Add(card);
        }
    }
}
