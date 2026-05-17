# M5 Asset Integration Plan

## Status: In Progress

## Completed
- [x] Created M5AssetConfigurator editor tool for building/sect/terrain configuration
- [x] Created PresetMapData ScriptableObject for preset maps
- [x] Created PresetMapCreator editor tool for generating preset maps
- [x] Modified HexGridManager to support loading preset maps
- [x] Created 3 preset maps: Jianghu, Spirit Realm, Wasteland
- [x] Balance analysis document (Docs/M5BalanceAnalysis.md)

## Asset Configuration Values (from GDD + Balance Analysis)

### Disciple Promotion Costs
| Transition | Tael | Qi | Lumber | Iron | Jade | Spirit Herbs | Turns |
|------------|------|-----|--------|------|------|--------------|-------|
| Peon → Outer | 10 | 0 | 0 | 0 | 0 | 0 | 6 |
| Outer → Inner | 30 | 10 | 0 | 2 | 0 | 0 | 10 |
| Inner → Elder | 80 | 40 | 0 | 0 | 2 | 0 | 18 |
| Elder → High Elder | 200 | 80 | 0 | 0 | 0 | 3 | 30 |

### Building Costs (T1 → T2 → T3)
| Building | T1 Cost | T2 Cost | T3 Cost | Turns |
|----------|---------|---------|---------|-------|
| Training Grounds | 50T+20L | 120T+50L+10Fe | 250T+20L+30Fe | 4/6/8 |
| Disciple Hall | 80T+10Q+30L | 180T+25Q+60L+15Fe | 350T+50Q+120L+40Fe | 6/10/14 |
| Library | 100T+20Q+40L | 200T+45Q+80L | 400T+90Q+150L+5J | 8/12/18 |
| Elder Council | 150T+30Q+50L+2J | 300T+60Q+100L+5J | 600T+120Q+200L+10J+5SH | 10/16/24 |
| External Affairs | 60T+25L | 140T+10Q+55L | 300T+25Q+110L+20Fe | 5/8/12 |
| Medicine Hall | 70T+5Q+20L+10MH | 160T+15Q+45L+25MH | 350T+35Q+90L+50MH+5SH | 5/9/14 |
| Armory | 60T+15L+15Fe | 140T+35L+40Fe | 300T+10Q+70L+80Fe+3J | 5/9/14 |
| Market Pavilion | 80T+30L | 180T+5Q+65L | 380T+15Q+130L+15Fe | 6/10/15 |
| Branch Sect Outpost | 200T+10Q+60L+10Fe | 400T+25Q+120L+25Fe | 800T+50Q+250L+50Fe+5J | 12/18/26 |
| Dao Sanctum | 500T+200Q+100L+50Fe+20J+10SH | — | — | 20 |

### Starting Resources
- Tael: 60 (was 100)
- Qi: 15 (was 20)
- Peons: 3 (was 5)
- Outer Disciples: 1 (was 0)

### Sect Configs (10 canonical sects)
All configured with:
- Affinity, Trait, Unique Hall
- Starting resources (60T/15Q/3Peons/1Outer)
- Banner colors
- Unique hall bonuses

## Remaining Tasks
- [ ] Run M5AssetConfigurator in Unity to generate all assets
- [ ] Assign meshes to BuildingConfigSO assets
- [ ] Assign materials to buildings
- [ ] Set up building prefabs with correct mesh references
- [ ] Set up unit prefabs with correct mesh references
- [ ] Configure M5VisualTest scene with preset map
- [ ] Wire up sect founding with Temple placement
- [ ] Play test all three preset maps
- [ ] Balance tuning based on play test feedback

## Preset Maps

### 1. Jianghu (Mainland China)
- Size: Large (120×80, 9,600 tiles)
- 7 Sacred Peaks: Kunlun, Hua, Wudang, Emei, Qingcheng, Shaolin
- 12 Settlements: Chang'an, Luoyang, Chengdu, Nanjing, Hangzhou, Beijing, Guangzhou, Lanzhou, Kunming, Shaoguan, Qingdao, Wuhan
- 4 Starting positions
- Geography: Tibet plateau (west), Gobi desert (northwest), Central Plains (center), Yangtze River, southern forests, eastern coast

### 2. Spirit Realm (Fantasy)
- Size: Medium (100×60, 6,000 tiles)
- 6 Sacred Peaks (more abundant)
- 5 Settlements
- Higher baseline Qi density
- More forests and varied terrain

### 3. Wasteland (Post-Cataclysm)
- Size: Medium (80×60, 4,800 tiles)
- 3 Sacred Peaks (rare oases)
- 3 Settlements (survivor enclaves)
- Mostly desert and swamp
- Very low Qi baseline
- Sparse resources
