using System;
using UniNativeLinq;

namespace AoAndSugi.Game.Models
{
    public struct Turn
    {
        public TurnId Id;
        public Board Board;
        public NativeEnumerable<Power> Powers;
        public NativeEnumerable<EnergySupplier> EnergySuppliers;

        public ref Power this[PowerId id] => ref Powers[id.Value];

        public unsafe void CopyToDeep(ref Turn value)
        {
            if (!Board.CopyTo(ref value.Board))
            {
#if DEBUG
                throw new NullReferenceException();
#endif
            }
            var end = 20L;
            if (end > Powers.Length)
                end = Powers.Length;
            if (end > value.Powers.Length)
                end = value.Powers.Length;
            for (var i = 0L; i != end; i++)
            {
                Powers[i].CopyTo(ref value.Powers[i]);
            }
            value.EnergySuppliers.ReAlloc(EnergySuppliers.Length);
            EnergySuppliers.CopyTo(value.EnergySuppliers.Ptr);
        }
    }
}