using System;
using NUnit.Framework;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using UnityEngine;

namespace TalesOfTao.Tests.Core
{
    // EditMode tests — no scene required, runs in the Unity Test Runner.
    public class EventChannelTests
    {
        private GamePhaseEventChannelSO _channel;

        [SetUp]
        public void SetUp()
        {
            _channel = ScriptableObject.CreateInstance<GamePhaseEventChannelSO>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_channel);
        }

        [Test]
        public void Raise_WithSubscriber_SubscriberReceivesValue()
        {
            GamePhase received = default;
            _channel.Subscribe(p => received = p);

            _channel.Raise(GamePhase.Action);

            Assert.AreEqual(GamePhase.Action, received);
        }

        [Test]
        public void Raise_MultipleSubscribers_AllReceiveValue()
        {
            int callCount = 0;
            _channel.Subscribe(_ => callCount++);
            _channel.Subscribe(_ => callCount++);
            _channel.Subscribe(_ => callCount++);

            _channel.Raise(GamePhase.Income);

            Assert.AreEqual(3, callCount);
        }

        [Test]
        public void Unsubscribe_ListenerNotCalledAfterRemoval()
        {
            int callCount = 0;
            Action<GamePhase> listener = _ => callCount++;
            _channel.Subscribe(listener);
            _channel.Unsubscribe(listener);

            _channel.Raise(GamePhase.Build);

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Subscribe_SameListenerTwice_OnlyCalledOnce()
        {
            int callCount = 0;
            Action<GamePhase> listener = _ => callCount++;
            _channel.Subscribe(listener);
            _channel.Subscribe(listener); // duplicate

            _channel.Raise(GamePhase.Research);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Raise_ListenerUnsubscribesDuringCallback_NoException()
        {
            // A listener that removes itself during the raise — backwards iteration makes this safe.
            Action<GamePhase> selfRemovingListener = null;
            selfRemovingListener = _ => _channel.Unsubscribe(selfRemovingListener);
            _channel.Subscribe(selfRemovingListener);

            Assert.DoesNotThrow(() => _channel.Raise(GamePhase.Resolution));
        }

        [Test]
        public void VoidChannel_Raise_SubscriberCalled()
        {
            var voidChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            bool called = false;
            voidChannel.Subscribe(() => called = true);

            voidChannel.Raise();

            Assert.IsTrue(called);
            UnityEngine.Object.DestroyImmediate(voidChannel);
        }
    }
}
