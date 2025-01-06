using Game.Data;
using Newtonsoft.Json;
using SadUtils;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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

            ProjectData projectData = await DeserializeAsync(fileName);

            GridManager.Instance.LoadRootGrid(projectData.gridData);

            isLoading = false;

            UnityEngine.Debug.Log($"Loaded Circuit in {timer.ElapsedMilliseconds}ms!");
            timer.Stop();
        }

        private Task<ProjectData> DeserializeAsync(string fileName)
        {
            return Task.Run(() =>
            {
                string json = LoadFileContents(fileName);

                try
                {
                    return JsonConvert.DeserializeObject<ProjectData>(
                        json,
                        SerializationManager.GetAllConverters());
                }
                catch (System.Exception e)
                {
                    throw e;
                }
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
    }
}
