using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class UpdatePackageRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
