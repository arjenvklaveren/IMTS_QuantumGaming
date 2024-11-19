using Game.Data;
using SadUtils;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class SerializationManager : Singleton<SerializationManager>
    {
        public const string saveDirectory = "/SaveData/Circuits";

        private bool isSaving;

        protected override void Awake()
        {
            SetInstance(this);
        }

        #region Serialize Grid
        // This function is REALLY slow. Consider looking into multithreading or making a loading graphic.
        public void SerializeGrid(GridData grid)
        {
            if (isSaving)
                return;

            Debug.Log("Saving File...");

            isSaving = true;

            // Convert data to Json.
            string jsonString = JsonConvert.SerializeObject(
                grid,
                GetAllConverters());

            Debug.Log("Serialized Data!");

            SaveFile(jsonString, grid.gridName);

            isSaving = false;

            Debug.Log("Finished Saving File!");
        }

        public static JsonConverter[] GetAllConverters()
        {
            return new JsonConverter[]
            {
                // simple types
                new Vector2JsonConverter(),
                new Vector2IntJsonConverter(),

                // optics types
                new GridDataJsonConverter(),
                new OpticComponentJsonConverter(),
                new ComponentPortJsonConverter(),
            };
        }

        private void SaveFile(string json, string gridName)
        {
            // Find file path.
            string filePath = GetFilePath(gridName);

            // Write to save file.
            using FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            // Write to file.
            using StreamWriter writer = new(fileStream);

            // Discard current file.
            fileStream.SetLength(0);

            // Write new Data.
            writer.Write(json);
        }

        public static string GetFilePath(string fileName, bool addExtension = true)
        {
            string filePath = $"{Application.dataPath}{saveDirectory}/{fileName}";

            if (addExtension)
                filePath += ".json";

            return filePath;
        }
        #endregion

        // TEST
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                SerializeGrid(GridManager.Instance.GetActiveGrid());
        }
    }
}
