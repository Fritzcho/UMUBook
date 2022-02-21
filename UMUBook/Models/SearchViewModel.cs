using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class SearchViewModel
    {
        public IEnumerable<UMUBook.Models.ProfileModel> Profiles { get; set; }
    }
}
