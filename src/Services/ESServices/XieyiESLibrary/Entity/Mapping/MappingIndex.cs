using System;
using System.Collections.Generic;

namespace XieyiESLibrary.Entity.Mapping
{
    public class MappingIndex
    {
        public Type Type { get; set; }

        public string IndexName { get; set; }

        public List<MappingColumn> Columns { get; } = new(0);
    }
}