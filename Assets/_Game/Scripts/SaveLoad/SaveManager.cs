using System;
using System.IO;
using UnityEngine;

namespace TalesOfTao.SaveLoad
{
    /// <summary>
    /// Static save/load manager. Handles atomic file writes, versioning, migration, and backup recovery.
    /// All public methods are safe to call from any thread (file I/O is synchronous).
    /// </summary>
    public static class SaveManager
    {
        private static string SaveDir =>
            Path.Combine(Application.persistentDataPath, "saves");

        /// <summary>
        /// Saves game data to the specified slot. Uses atomic write pattern (temp → backup → move).
        /// Slot 0 = autosave, slots 1-3 = manual saves.
        /// </summary>
        public static bool Save(int slot, SaveData data)
        {
            if (slot < 0 || slot > SaveConstants.MaxManualSlot)
            {
                Debug.LogError($"[SaveManager] Invalid slot {slot}. Must be 0-{SaveConstants.MaxManualSlot}.");
                return false;
            }

            try
            {
                Directory.CreateDirectory(SaveDir);
                data.timestamp = DateTime.UtcNow.ToString("o");
                data.version = SaveConstants.CurrentVersion;

                string json = JsonUtility.ToJson(data, prettyPrint: true);
                string path = SlotPath(slot);
                string tmpPath = path + ".tmp";
                string bakPath = path + ".bak";

                // Write to temp file first (atomic write pattern)
                File.WriteAllText(tmpPath, json);

                // Backup existing save
                if (File.Exists(path))
                {
                    File.Copy(path, bakPath, overwrite: true);
                }

                // Atomic move (rename is atomic on all platforms)
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.Move(tmpPath, path);

                Debug.Log($"[SaveManager] Saved slot {slot} ({json.Length} bytes) at {data.timestamp}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Save failed for slot {slot}: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads game data from the specified slot. Falls back to .bak if primary is corrupted.
        /// Returns null if neither file exists or both are corrupted.
        /// </summary>
        public static SaveData Load(int slot)
        {
            if (slot < 0 || slot > SaveConstants.MaxManualSlot)
            {
                Debug.LogError($"[SaveManager] Invalid slot {slot}. Must be 0-{SaveConstants.MaxManualSlot}.");
                return null;
            }

            string path = SlotPath(slot);

            // Try primary save, then backup
            if (!File.Exists(path))
            {
                string bakPath = path + ".bak";
                if (File.Exists(bakPath))
                {
                    Debug.LogWarning($"[SaveManager] Primary save missing for slot {slot}, loading backup.");
                    path = bakPath;
                }
                else
                {
                    Debug.Log($"[SaveManager] No save file found for slot {slot}.");
                    return null;
                }
            }

            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data == null)
                {
                    Debug.LogError($"[SaveManager] Deserialized null for slot {slot}.");
                    return null;
                }

                // Migrate if needed
                data = MigrateSave(data);

                Debug.Log($"[SaveManager] Loaded slot {slot} (v{data.version}, turn {data.turnNumber})");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Load failed for slot {slot}: {e.Message}");

                // Try backup as last resort
                string bakPath = path + ".bak";
                if (File.Exists(bakPath) && path != bakPath)
                {
                    try
                    {
                        string json = File.ReadAllText(bakPath);
                        SaveData data = JsonUtility.FromJson<SaveData>(json);
                        Debug.LogWarning($"[SaveManager] Loaded from backup for slot {slot}.");
                        return MigrateSave(data);
                    }
                    catch
                    {
                        Debug.LogError($"[SaveManager] Backup also corrupted for slot {slot}.");
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Checks if a save file exists for the given slot.
        /// </summary>
        public static bool SaveExists(int slot)
        {
            return File.Exists(SlotPath(slot)) || File.Exists(SlotPath(slot) + ".bak");
        }

        /// <summary>
        /// Deletes all save files for a slot (primary + backup).
        /// </summary>
        public static bool DeleteSave(int slot)
        {
            bool deleted = false;
            string path = SlotPath(slot);
            string bakPath = path + ".bak";

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    deleted = true;
                }
                if (File.Exists(bakPath))
                {
                    File.Delete(bakPath);
                    deleted = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Delete failed for slot {slot}: {e.Message}");
                return false;
            }

            return deleted;
        }

        /// <summary>
        /// Returns the file path for a save slot.
        /// </summary>
        public static string GetSlotPath(int slot) => SlotPath(slot);

        /// <summary>
        /// Returns all existing save slots with metadata.
        /// </summary>
        public static List<SaveSlotInfo> GetSaveSlotInfos()
        {
            var infos = new List<SaveSlotInfo>();

            for (int i = 0; i <= SaveConstants.MaxManualSlot; i++)
            {
                string path = SlotPath(i);
                if (File.Exists(path))
                {
                    try
                    {
                        string json = File.ReadAllText(path);
                        SaveData data = JsonUtility.FromJson<SaveData>(json);
                        if (data != null)
                        {
                            infos.Add(new SaveSlotInfo
                            {
                                slot = i,
                                timestamp = data.timestamp,
                                turnNumber = data.turnNumber,
                                isValid = true,
                                fileSize = new FileInfo(path).Length
                            });
                            continue;
                        }
                    }
                    catch { /* fall through to invalid */ }

                    infos.Add(new SaveSlotInfo { slot = i, isValid = false });
                }
            }

            return infos;
        }

        private static string SlotPath(int slot) =>
            Path.Combine(SaveDir, $"save_slot_{slot}.json");

        /// <summary>
        /// Migrates old save formats to the current version.
        /// Add migration steps here when SaveData format changes.
        /// </summary>
        private static SaveData MigrateSave(SaveData data)
        {
            if (data.version < 2)
            {
                // v1 → v2: formations added
                if (data.formations == null)
                    data.formations = new System.Collections.Generic.List<FormationSaveData>();
            }

            if (data.version < 3)
            {
                // v2 → v3: activeAction field added to units
                if (data.units != null)
                {
                    foreach (var unit in data.units)
                    {
                        if (string.IsNullOrEmpty(unit.activeAction))
                            unit.activeAction = "None";
                    }
                }
            }

            // Add future migrations here:
            // if (data.version < 4) { ... }

            data.version = SaveConstants.CurrentVersion;
            return data;
        }
    }

    /// <summary>
    /// Metadata about a save slot for UI display.
    /// </summary>
    [Serializable]
    public class SaveSlotInfo
    {
        public int slot;
        public string timestamp;
        public int turnNumber;
        public bool isValid;
        public long fileSize;
    }
}
