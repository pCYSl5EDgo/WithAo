using AoAndSugi.Game.Models;
using UniNativeLinq;

namespace Game.Models
{
    public unsafe struct Turn
    {
        public NativeEnumerable<Power> Powers;
    }
}