using UnityEngine;

namespace Game.Controllers
{
    public class ChessplaneBuilder
    {
        private readonly Quaternion _plateRotation = Quaternion.Euler(90, 0, 0);

        private GameObject[] _blocks;
        private GameObject _parentObject;

        public ChessplaneBuilder(GameObject[] blockVariations, Vector2Int size, float height = 0)
        {
            _parentObject = new GameObject();
            _blocks = new GameObject[size.x * size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject selectedPrefab = blockVariations[(x + y) % blockVariations.Length];
                    GameObject block = GameObject.Instantiate(blockVariations[(x + y) % blockVariations.Length], new Vector3(x - 0.5f * size.x + 0.5f, height, y - 0.5f * size.y + 0.5f), _plateRotation);
                    block.transform.SetParent(_parentObject.transform);
                }
            }
        }

        public Vector3 Position
        {
            set
            {
                _parentObject.transform.position = new Vector3(value.x, 0, value.z);
            }
        }

        public void SetActive(bool value)
        {
            _parentObject.gameObject.SetActive(value);
        }

        public void Destroy()
        {
            foreach (var block in _blocks)
            {
                GameObject.Destroy(block);
            }
            GameObject.Destroy(_parentObject);
        }
    }
}