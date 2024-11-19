using Game.Data;
using SadUtils;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class DeserializationManager : Singleton<DeserializationManager>
    {
        private bool isLoading;

        protected override void Awake()
        {
            SetInstance(this);
        }

        #region Deserialize
        public void LoadCircuit(string fileName)
        {
            Debug.Log("Loading Circuit...");

            if (isLoading)
                return;

            isLoading = true;

            string json = LoadFileContents(fileName);

            GridData grid = JsonConvert.DeserializeObject<GridData>(
                json,
                SerializationManager.GetAllConverters());

            GridManager.Instance.LoadRootGrid(grid);

            isLoading = false;

            Debug.Log("Loaded Circuit!");
        }

        private string LoadFileContents(string fileName)
        {
            string filePath = SerializationManager.GetFilePath(fileName, false);

            using FileStream fileStream = File.OpenRead(filePath);
            using StreamReader reader = new(fileStream);

            return reader.ReadToEnd();
        }
        #endregion

        // TEST
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
                LoadCircuit("testGrid.json");
        }
    }
}
