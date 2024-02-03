using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DpeZak.Kmp;

namespace DpeZak.Entities.DPE
{
    public partial class FLTR
    {
        //berechnete Eigenschaften
        //keine Spaltennamen (auch nicht als Lowercase)! Deshalb Prefix 'cf':
        [NotMapped]
        public ColumnList cfColumnlist { get => new(COLUMNLIST); }

        [NotMapped]
        public FltrList cfFltrlist { get => new(FLTRLIST); }
    }
}
