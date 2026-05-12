using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.Pooling
{
    // Generic object pool for any reference type.
    // For GameObject pooling, use the concrete GameObjectPool below.
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T>  _pool = new();
        private readonly Func<T>   _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;

#if UNITY_EDITOR || DEBUG
        private readonly HashSet<T> _activeItems = new();
#endif

        public int CountInactive => _pool.Count;
        public int CountAll      { get; private set; }

        public ObjectPool(
            Func<T>   factory,
            Action<T> onGet        = null,
            Action<T> onReturn     = null,
            int       prewarmCount = 0)
        {
            _factory  = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet    = onGet;
            _onReturn = onReturn;

            for (int i = 0; i < prewarmCount; i++)
            {
                _pool.Push(_factory());
                CountAll++;
            }
        }

        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : Create();
#if UNITY_EDITOR || DEBUG
            _activeItems.Add(item);
#endif
            _onGet?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
#if UNITY_EDITOR || DEBUG
            if (!_activeItems.Remove(item))
                throw new InvalidOperationException(
                    $"[ObjectPool<{typeof(T).Name}>] Item not from this pool or already returned.");
#endif
            _onReturn?.Invoke(item);
            _pool.Push(item);
        }

        public void Clear()
        {
            CountAll -= _pool.Count;
            _pool.Clear();
        }

        private T Create() { CountAll++; return _factory(); }
    }

    /// <summary>
    /// Unity GameObject pool — wraps ObjectPool with Instantiate/Destroy.
    /// </summary>
    /// <example>
    /// <code>
    /// var pool = new GameObjectPool(enemyPrefab, prewarmCount: 10);
    /// var enemy = pool.Get(spawnPoint.position, Quaternion.identity);
    /// pool.Return(enemy);
    /// </code>
    /// </example>
    public class GameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly ObjectPool<GameObject> _inner;
        private readonly Transform _parent;

        public int CountInactive => _inner.CountInactive;
        public int CountAll => _inner.CountAll;

        /// <param name="prefab">Prefab to pool (must not be active in scene).</param>
        /// <param name="parent">Optional parent transform. Null = scene root.</param>
        /// <param name="prewarmCount">Number to pre-instantiate.</param>
        public GameObjectPool(GameObject prefab, Transform parent = null, int prewarmCount = 0)
        {
            _prefab = prefab;
            _parent = parent;
            _inner = new ObjectPool<GameObject>(
                factory: Create,
                onGet: go => go.SetActive(true),
                onReturn: go =>
                {
                    go.SetActive(false);
                    if (_parent != null)
                        go.transform.SetParent(_parent);
                },
                prewarmCount: prewarmCount
            );
        }

        public GameObject Get(Vector3 position = default, Quaternion rotation = default)
        {
            var go = _inner.Get();
            go.transform.SetPositionAndRotation(position, rotation);
            return go;
        }

        public void Return(GameObject go)
        {
            if (go == null) return;
            _inner.Return(go);
        }

        public void Clear()
        {
            foreach (var go in _inner)
            {
                // ObjectPool doesn't expose active items; this is handled internally
            }
            _inner.Clear();
        }

        private GameObject Create()
        {
            var go = UnityEngine.Object.Instantiate(_prefab);
            go.SetActive(false);
            if (_parent != null)
                go.transform.SetParent(_parent);
            return go;
        }
    }
}
