using System;
using NUnit.Framework;
using TalesOfTao.Core.Pooling;

namespace TalesOfTao.Tests.Core
{
    public class ObjectPoolTests
    {
        // ── Construction ──────────────────────────────────────────────────────

        [Test]
        public void Constructor_NullFactory_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectPool<object>(factory: null));
        }

        [Test]
        public void Prewarm_CreatesExactCount()
        {
            int factoryCalls = 0;
            var pool = new ObjectPool<object>(
                factory: () => { factoryCalls++; return new object(); },
                prewarmCount: 5);

            Assert.AreEqual(5, factoryCalls);
            Assert.AreEqual(5, pool.CountInactive);
            Assert.AreEqual(5, pool.CountAll);
        }

        // ── Get ───────────────────────────────────────────────────────────────

        [Test]
        public void Get_PrewarmedPool_DoesNotAllocate()
        {
            int factoryCalls = 0;
            var pool = new ObjectPool<object>(
                factory: () => { factoryCalls++; return new object(); },
                prewarmCount: 3);

            pool.Get();

            // Factory was called 3× during prewarm, 0× during Get.
            Assert.AreEqual(3, factoryCalls);
        }

        [Test]
        public void Get_EmptyPool_AllocatesViaFactory()
        {
            int factoryCalls = 0;
            var pool = new ObjectPool<object>(factory: () => { factoryCalls++; return new object(); });

            pool.Get();

            Assert.AreEqual(1, factoryCalls);
            Assert.AreEqual(1, pool.CountAll);
        }

        [Test]
        public void Get_InvokesOnGetCallback()
        {
            bool callbackFired = false;
            var pool = new ObjectPool<object>(
                factory: () => new object(),
                onGet: _ => callbackFired = true,
                prewarmCount: 1);

            pool.Get();

            Assert.IsTrue(callbackFired);
        }

        // ── Return ────────────────────────────────────────────────────────────

        [Test]
        public void Return_ThenGet_ReturnsSameInstance()
        {
            var pool = new ObjectPool<object>(factory: () => new object(), prewarmCount: 1);
            var first = pool.Get();
            pool.Return(first);
            var second = pool.Get();

            Assert.AreSame(first, second);
        }

        [Test]
        public void Return_NullItem_ThrowsArgumentNullException()
        {
            var pool = new ObjectPool<object>(factory: () => new object());
            Assert.Throws<ArgumentNullException>(() => pool.Return(null));
        }

        [Test]
        public void Return_InvokesOnReturnCallback()
        {
            bool callbackFired = false;
            var pool = new ObjectPool<object>(
                factory: () => new object(),
                onReturn: _ => callbackFired = true);

            var item = pool.Get();
            pool.Return(item);

            Assert.IsTrue(callbackFired);
        }

        // ── Count tracking ────────────────────────────────────────────────────

        [Test]
        public void CountInactive_TracksPoolSize()
        {
            var pool = new ObjectPool<object>(factory: () => new object(), prewarmCount: 4);
            Assert.AreEqual(4, pool.CountInactive);

            var a = pool.Get();
            var b = pool.Get();
            Assert.AreEqual(2, pool.CountInactive);

            pool.Return(a);
            Assert.AreEqual(3, pool.CountInactive);
        }
    }
}
