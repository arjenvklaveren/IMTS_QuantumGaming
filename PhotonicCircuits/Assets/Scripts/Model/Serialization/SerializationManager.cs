using Game.Data;
using SadUtils;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class SerializationManager : Singleton<SerializationManager>
    {
        protected override void Awake()
        {
            SetInstance(this);
        }

        #region Serialize Grid
        public void SerializeGrid(GridData grid)
        {
            string fileName = $"{grid.gridName}.json";

            string jsonString = JsonConvert.SerializeObject(
                grid,
                GetAllConverters());

            Debug.Log(jsonString);
            // Write to savefile
        }

        private JsonConverter[] GetAllConverters()
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
        #endregion

        // TEST
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                SerializeGrid(GridManager.Instance.GetActiveGrid());
        }
    }
}
