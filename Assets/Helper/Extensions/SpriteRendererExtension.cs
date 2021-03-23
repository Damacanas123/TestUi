using UnityEngine;

namespace Slime.Helper.Extensions
{
    public static class SpriteRendererExtension
    {
        public static int CompareTo(this SpriteRenderer self, SpriteRenderer other)
        {
            
            int selfLayer = SortingLayer.GetLayerValueFromID(self.sortingLayerID);
            int otherLayer = SortingLayer.GetLayerValueFromID(other.sortingLayerID);
            int layerCompare = selfLayer.CompareTo(otherLayer);
            if (layerCompare == 0)
            {
                return self.sortingOrder.CompareTo(other.sortingOrder);
            }

            return layerCompare;
        }
    }
}