using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IItemsRepository
    {
        //get all items
        List<IItemValidating> Get();

        //save the items
        void Save(List<IItemValidating> items);
    }
}
