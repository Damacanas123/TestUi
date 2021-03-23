using System.Collections.Generic;
using UnityEngine;

namespace Slime.Helper.GameHelper
{
    public interface IMatrix<T> where T : ACellModel
    {
        void SetDimensions(int width, int height);
        T GetItem(int x, int y);
        void SetItem(int x, int y, T item);
        T[] GetRow(int y);
        T[] GetColumn(int x);
        //slide items in a circular manner
        void OffsetRow(int y, int offset);
        void OffsetColumn(int x, int offset);
        /// <summary>
        /// Takes coordinates of starting cell and returns the unoccupied maximum rectangle that contains given cell type (left,top,right,down). Marks returned rectnagle as occupied
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cellType"></param>
        /// <returns></returns>
        (int, int,int,int) GetMaxRectangleBox(int x, int y);

        void Print();

        List<Vector2> GetSimilarRegion(int x, int y);

        T[] GetPlusShape(int x, int y);
    }
}