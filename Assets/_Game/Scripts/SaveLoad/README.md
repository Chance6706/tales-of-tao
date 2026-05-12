# Save/Load Module (Phase 4)

Planned systems:
- `SaveManager` — serializes game state to JSON (tiles, units, sects, resources)
- `LoadManager` — deserializes and reconstructs game state via CommandQueue.Replay
- `SaveFileValidator` — schema validation + migration support for save format versions

See GDD v2 Section 5.1 for save format specification.
