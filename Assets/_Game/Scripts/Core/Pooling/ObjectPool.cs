using System;
using System.Collections.Generic;

namespace TalesOfTao.Core.Pooling
{
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
}
