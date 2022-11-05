using System;
using Maintenance.Application.Helpers.GenericSort.Enums;

namespace Maintenance.Application.Helpers.GenericSort
{
    public class SortingParams
    {
        public SortOrders SortOrder { get; set; } = SortOrders.Asc;
        public string ColumnName { get; set; }
    }
}
