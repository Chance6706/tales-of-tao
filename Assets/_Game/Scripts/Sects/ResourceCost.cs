using System;
using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Resource cost for building construction or disciple recruitment.
    /// All seven commodity types plus Tael and Qi.
    /// </summary>
    [Serializable]
    public struct ResourceCost
    {
        public int Tael;
        public int Qi;
        public int Lumber;
        public int IronOre;
        public int Jade;
        public int MedicinalHerbs;
        public int SpiritHerbs;

        /// <summary>
        /// Returns true if all cost components are zero or negative.
        /// </summary>
        public bool IsFree => Tael <= 0 && Qi <= 0 && Lumber <= 0 && IronOre <= 0
            && Jade <= 0 && MedicinalHerbs <= 0 && SpiritHerbs <= 0;

        /// <summary>
        /// Checks if the given stockpile can afford this cost.
        /// </summary>
        public bool CanAfford(ResourceStockpile stockpile)
        {
            return stockpile.Tael >= Tael
                && stockpile.Qi >= Qi
                && stockpile.Lumber >= Lumber
                && stockpile.IronOre >= IronOre
                && stockpile.Jade >= Jade
                && stockpile.MedicinalHerbs >= MedicinalHerbs
                && stockpile.SpiritHerbs >= SpiritHerbs;
        }

        /// <summary>
        /// Deducts this cost from the stockpile. Assumes CanAfford was checked.
        /// </summary>
        public void DeductFrom(ref ResourceStockpile stockpile)
        {
            stockpile.Tael -= Tael;
            stockpile.Qi -= Qi;
            stockpile.Lumber -= Lumber;
            stockpile.IronOre -= IronOre;
            stockpile.Jade -= Jade;
            stockpile.MedicinalHerbs -= MedicinalHerbs;
            stockpile.SpiritHerbs -= SpiritHerbs;
        }

        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (Tael > 0) parts.Add($"{Tael}T");
            if (Qi > 0) parts.Add($"{Qi}Q");
            if (Lumber > 0) parts.Add($"{Lumber}L");
            if (IronOre > 0) parts.Add($"{IronOre}Fe");
            if (Jade > 0) parts.Add($"{Jade}J");
            if (MedicinalHerbs > 0) parts.Add($"{MedicinalHerbs}MH");
            if (SpiritHerbs > 0) parts.Add($"{SpiritHerbs}SH");
            return string.Join("/", parts);
        }
    }
}
