
using UnityEngine;

namespace Helper.Context
{
    public class GameContext : MonoBehaviour
    {
        //instantiate level or container objects here
        [SerializeField] private GameObject[] instantiatePrefabs;
        
        void Awake()
        {
            foreach (var prefab in instantiatePrefabs)
            {
                Instantiate(prefab);
            }
        }
    }
}
