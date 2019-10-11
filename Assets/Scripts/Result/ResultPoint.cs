using System;

namespace AoAndSugi.Result
{
    [Serializable]
    public struct ResultPoint : IEquatable<ResultPoint>
    {
        public int Value;

        public ResultPoint(int value) => Value = value;

        public bool Equals(ResultPoint other) => Value == other.Value;

        public override bool Equals(object obj) => obj is ResultPoint other && Equals(other);

        public override int GetHashCode() => Value;

        public override string ToString() => Value.ToString();
    }
}