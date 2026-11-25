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
        private const string AllCategories = "Все категории";
        private const string AllSubcategories = "Все подкатегории";

        private readonly List<ShoeCardViewModel> _allCards = new();
        private readonly UserSession _session;
        private string _searchText = string.Empty;
        private string _selectedCategory = AllCategories;
        private string _selectedSubcategory = AllSubcategories;
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
            Categories.Insert(0, AllCategories);

            RefreshSubcategories();

            ApplyFilters();
        }

        public ObservableCollection<ShoeCardViewModel> Cards { get; } = new();
        public List<string> Categories { get; }
        private List<string> _subcategories = new();
        public List<string> Subcategories
        {
            get => _subcategories;
            private set => SetProperty(ref _subcategories, value);
        }

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
            set
            {
                if (!SetProperty(ref _selectedCategory, value))
                    return;

                RefreshSubcategories();
                _selectedSubcategory = AllSubcategories;
                OnPropertyChanged(nameof(SelectedSubcategory));
                ApplyFilters();
            }
        }

        public string SelectedSubcategory
        {
            get => _selectedSubcategory;
            set { if (SetProperty(ref _selectedSubcategory, value)) ApplyFilters(); }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set { if (SetProperty(ref _selectedSort, value)) ApplyFilters(); }
        }

        private void RefreshSubcategories()
        {
            IEnumerable<ShoeCardViewModel> scope = _allCards;

            if (SelectedCategory != AllCategories)
                scope = scope.Where(c => c.CategoryName == SelectedCategory);

            var subs = scope
                .Select(c => c.SubcategoryName)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            subs.Insert(0, AllSubcategories);
            Subcategories = subs;
        }

        private void ApplyFilters()
        {
            IEnumerable<ShoeCardViewModel> query = _allCards;

            if (SelectedCategory != AllCategories)
                query = query.Where(c => c.CategoryName == SelectedCategory);

            if (SelectedSubcategory != AllSubcategories)
                query = query.Where(c => c.SubcategoryName == SelectedSubcategory);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                query = query.Where(c =>
                    c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
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
