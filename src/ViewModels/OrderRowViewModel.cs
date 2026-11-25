using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.ViewModels
{
    public class OrderRowViewModel
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string UserFullName { get; init; } = "";
        public float TotalPrice { get; init; }
    }
}
