using Game.Data;
using SadUtils;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            if (isLoading)
                return;

            LoadCircuitContents(fileName);
        }

        private async void LoadCircuitContents(string fileName)
        {
            UnityEngine.Debug.Log("Loading Circuit...");
            Stopwatch timer = new();
            timer.Start();

            isLoading = true;

            GridData grid = await DeserializeAsync(fileName);

            GridManager.Instance.LoadRootGrid(grid);

            isLoading = false;

            UnityEngine.Debug.Log($"Loaded Circuit in {timer.ElapsedMilliseconds}ms!");
            timer.Stop();
        }

        private Task<GridData> DeserializeAsync(string fileName)
        {
            return Task.Run(() =>
            {
                string json = LoadFileContents(fileName);

                return JsonConvert.DeserializeObject<GridData>(
                    json,
                    SerializationManager.GetAllConverters());
            });
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
