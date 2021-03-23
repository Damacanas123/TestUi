using System;

namespace Slime.Helper.GameHelper
{
    public abstract class ACellModel : IEquatable<ACellModel>
    {
        
        public bool isOccupied;

        public abstract bool Equals(ACellModel other);

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();
        public int x;
        public int y;
    }
}