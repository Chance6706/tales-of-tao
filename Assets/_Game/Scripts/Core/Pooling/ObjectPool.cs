using System;
using System.Collections.Generic;

namespace TalesOfTao.Core.Pooling
{
    // Generic object pool. Avoids GC pressure from frequent Instantiate/Destroy
    // on large maps (tile meshes, unit tokens, VFX particles, UI popups).
    //
    // Usage:
    //   var pool = new ObjectPool<MyClass>(factory: () => new MyClass(),
    //                                     onGet: obj => obj.Reset(),
    //                                     onReturn: obj => obj.Deactivate(),
    //                                     prewarmCount: 20);
    //   var obj = pool.Get();
    //   pool.Return(obj);
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool = new();
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;

        public int CountInactive => _pool.Count;
        public int CountAll { get; private set; }

        public ObjectPool(
            Func<T> factory,
            Action<T> onGet = null,
            Action<T> onReturn = null,
            int prewarmCount = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onReturn = onReturn;

            for (int i = 0; i < prewarmCount; i++)
            {
                _pool.Push(_factory());
                CountAll++;
            }
        }

        // Returns a pooled instance (or a freshly created one if the pool is empty).
        public T Get()
        {
            T item;
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
            }
            else
            {
                item = _factory();
                CountAll++;
            }

            _onGet?.Invoke(item);
            return item;
        }

        // Returns an item to the pool. The caller must not use the object after this call.
        public void Return(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _onReturn?.Invoke(item);
            _pool.Push(item);
        }

        public void Clear() => _pool.Clear();
    }
}
