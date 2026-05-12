namespace TalesOfTao.Units
{
    // Cultivation tier for a unit. Values are pinned — save data stores these as ints.
    // `Invalid = 0` is reserved and should never be persisted; it serves as a safe
    // default / sentinel value when deserializing uninitialized or corrupted data.
    public enum SectTier : int
    {
        Invalid        = 0, // sentinel — never persisted
        Novice         = 1, // T1 — basic disciple
        Disciple       = 2, // T2 — inner sect member
        Elder          = 3, // T3 — sect elder
        Patriarch      = 4, // T4 — sect patriarch
        GrandPatriarch = 5, // T5 — peak cultivator; triggers emission effect
    }
}
