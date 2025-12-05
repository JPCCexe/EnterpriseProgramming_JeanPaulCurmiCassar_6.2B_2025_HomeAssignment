using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IItemValidating
    {
        // Returns a list of validator identifiers
        List<string> GetValidators();

        // Returns the name of the partial view used to display this item in the catalog
        string GetCardPartial();
    }
}
