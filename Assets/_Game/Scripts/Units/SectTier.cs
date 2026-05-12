namespace TalesOfTao.Units
{
    // Cultivation tier for a unit. Values are pinned — save data stores these as ints.
    public enum SectTier : int
    {
        Novice         = 1, // T1 — basic disciple
        Disciple       = 2, // T2 — inner sect member
        Elder          = 3, // T3 — sect elder
        Patriarch      = 4, // T4 — sect patriarch
        GrandPatriarch = 5, // T5 — peak cultivator; triggers emission effect
    }
}
