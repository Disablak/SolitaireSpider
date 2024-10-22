using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly GameObject _poolItemPref;
        private readonly Transform _poolHolder;
        private readonly int _poolStartSize;
        private readonly Stack<T> _pool;
        private int _poolSize;

        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="itemPref">The prefab of the object to be pooled.</param>
        /// <param name="poolHolder">The transform to serve as the parent for the pooled objects.</param>
        /// <param name="startSize">The initial size of the object pool.</param>
        public ObjectPool(GameObject itemPref, Transform poolHolder, int startSize)
        {
            _poolItemPref = itemPref;
            _poolHolder = poolHolder;
            _poolStartSize = startSize;
            _poolSize = 0;
            _pool = new Stack<T>();
            Fill();
        }

        /// <summary>
        /// Fills the object pool to reach the specified start size.
        /// </summary>
        private void Fill()
        {
            if (_poolSize >= _poolStartSize) return;
            int step = _poolStartSize - _poolSize;
            for (int i = 0; i < step; i++)
            {
                SpawnNewItem();
            }
        }

        /// <summary>
        /// Spawns a new item from the prefab and adds it to the object pool.
        /// </summary>
        private void SpawnNewItem()
        {
            var item = Object.Instantiate(_poolItemPref, _poolHolder);
            Push(item);
        }

        /// <summary>
        /// Adds an item to the object pool.
        /// </summary>
        /// <param name="item">The GameObject to be added to the pool.</param>
        public void Push(GameObject item)
        {
            _poolSize++;
            _pool.Push(item.GetComponent<T>());
            item.SetActive(false);
            item.transform.SetParent(_poolHolder);
        }

        /// <summary>
        /// Retrieves an item from the object pool, creating a new one if the pool is empty.
        /// </summary>
        /// <returns>The retrieved item from the pool.</returns>
        public T Pull()
        {
            if (_poolSize <= 0)
                SpawnNewItem();

            _poolSize--;
            T item = _pool.Pop();
            item.gameObject.SetActive(true);
            return item;
        }
    }