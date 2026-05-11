using Common;
using UnityEngine;

namespace Mecha {
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_Prefabs;
        [SerializeField] private float m_SpawnTime;

        private Timer m_Timer;
        void Start()
        {
            // First spawn faster
            m_Timer = new Timer(m_SpawnTime / 2);
        }

        // Update is called once per frame
        void Update()
        {
            m_Timer.RemoveTime(Time.deltaTime);
            if (m_Timer.IsFinish)
            {
                int prefabIndex = Mathf.FloorToInt(Random.Range(0, m_Prefabs.Length));
                GameObject nextSpawn = Instantiate(m_Prefabs[prefabIndex], transform.position, Quaternion.identity);

                m_Timer.Start(m_SpawnTime);
            }
        }
    }
}