using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Units
{
    // Central registry of all unit archetypes in the project.
    // Assign this asset to any system that needs to look up units by name or index.
    //
    // Asset creation: Assets > Create > TalesOfTao > Units > Unit Registry
    // Save into Assets/_Game/Data/Units/.
    [CreateAssetMenu(menuName = "TalesOfTao/Units/Unit Registry", fileName = "UnitRegistry")]
    public class UnitRegistrySO : ScriptableObject
    {
        [SerializeField] private UnitDataSO[] _units = System.Array.Empty<UnitDataSO>();

        public IReadOnlyList<UnitDataSO> Units => _units;

        public UnitDataSO FindByName(string unitName)
        {
            foreach (var u in _units)
                if (u != null && u.UnitName == unitName) return u;
            return null;
        }
    }
}
