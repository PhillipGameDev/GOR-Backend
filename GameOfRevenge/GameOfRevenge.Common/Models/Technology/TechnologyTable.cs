using GameOfRevenge.Common.Interface.Model.Table;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models.Technology
{
    public interface IReadOnlyTechnologyTable : IReadOnlyBaseRefEnumTable<TechnologyType>
    {

    }

    public class TechnologyTable : BaseRefEnumTable<TechnologyType>, IBaseTable, IBaseRefEnumTable<TechnologyType>, IReadOnlyBaseRefEnumTable<TechnologyType>, IReadOnlyTechnologyTable
    {

    }



/*    public interface IReadOnlySubTechnologyTable : IReadOnlyBaseRefEnumTable<SubTechnologyType>
    {

    }

    public class SubTechnologyTable : BaseRefEnumTable<SubTechnologyType>, IBaseTable, IBaseRefEnumTable<SubTechnologyType>, IReadOnlyBaseRefEnumTable<SubTechnologyType>, IReadOnlySubTechnologyTable
    {
    }*/
}
