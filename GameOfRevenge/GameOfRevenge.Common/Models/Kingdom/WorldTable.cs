using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;
using System.Collections.Generic;
using System.Text;

namespace GameOfRevenge.Common.Models.Kingdom
{
    public class WorldTable : BaseRefStringTable, IBaseTable, IBaseRefEnumTable<string>, IReadOnlyBaseRefEnumTable<string>
    {

    }
}
