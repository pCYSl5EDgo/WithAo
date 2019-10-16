using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;

namespace AoAndSugi.Game.Models
{
    public readonly struct GameMasterData
    {
        private readonly NativeEnumerable<NativeEnumerable<UnitInitialHp>> initialHpTable;
        private readonly NativeEnumerable<NativeEnumerable<NativeEnumerable<UnitMovePower>>> movePowerTable;

        public GameMasterData(in NativeEnumerable<NativeEnumerable<UnitInitialHp>> initialHpTable, NativeEnumerable<NativeEnumerable<NativeEnumerable<UnitMovePower>>> movePowerTable)
        {
            this.initialHpTable = initialHpTable;
            this.movePowerTable = movePowerTable;
        }

        public ref readonly UnitInitialHp GetInitialHp(SpeciesType speciesType, UnitType unitType) 
            => ref initialHpTable[speciesType.Value][(int)unitType];

        public ref readonly UnitMovePower GetMovePower(SpeciesType speciesType, UnitType unitType, CellType cellType, bool isInItsRegion) 
            => ref movePowerTable[speciesType.Value][(int) unitType][(int) cellType * 2 + (isInItsRegion ? 1 : 0)];
    }
}