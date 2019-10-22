using AoAndSugi.Game.Models;

namespace AoAndSugi.Game.IO
{
    public interface IDrawUnit
    {
        void Draw(ref Turn turn);
    }
}