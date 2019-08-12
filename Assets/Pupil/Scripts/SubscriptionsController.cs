﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PupilLabs
{
    public partial class SubscriptionsController : MonoBehaviour
    {

        public PupilLabs.RequestController requestCtrl;

        public bool IsConnected { get { return requestCtrl.IsConnected; } }

        public delegate void ReceiveDataDelegate(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null);

        private Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();

        void OnEnable()
        {
            if (requestCtrl != null)
            {
                requestCtrl.OnDisconnecting += Disconnect;
            }
        }

        void OnDisable()
        {
            Disconnect();
        }

        void Update()
        {
            UpdateSubscriptionSockets();
        }

        public void Disconnect()
        {
            foreach (var socketKey in subscriptions.Keys)
                CloseSubscriptionSocket(socketKey);
            UpdateSubscriptionSockets();
        }

        public void SubscribeTo(string topic, ReceiveDataDelegate subscriberHandler)
        {
            if (!subscriptions.ContainsKey(topic))
            {
                string connectionStr = requestCtrl.GetSubConnectionString();
                Subscription subscription = new Subscription(connectionStr, topic);

                subscriptions.Add(topic, subscription);
                // subscriptions[topic].OnReceiveData += Logging; 
            }

            subscriptions[topic].OnReceiveData += subscriberHandler;
        }

        public void UnsubscribeFrom(string topic, ReceiveDataDelegate subscriberHandler)
        {
            if (subscriptions.ContainsKey(topic) && subscriberHandler != null)
            {
                subscriptions[topic].OnReceiveData -= subscriberHandler;

                if (!subscriptions[topic].HasSubscribers)
                {
                    CloseSubscriptionSocket(topic);
                }
            }
        }

        private void CloseSubscriptionSocket(string topic)
        {
            if (subscriptions.ContainsKey(topic))
            {
                subscriptions[topic].ShouldClose = true;
            }
        }

        private void UpdateSubscriptionSockets()
        {

            List<string> toBeRemoved = new List<string>();
            foreach (var subscription in subscriptions.Values.ToList())
            {
                if (!subscription.ShouldClose)
                {
                    subscription.UpdateSocket();
                }
                else
                {
                    subscription.Close();
                    toBeRemoved.Add(subscription.topic);
                }
            }

            foreach (var removeTopic in toBeRemoved)
            {
                if (subscriptions.ContainsKey(removeTopic))
                {
                    subscriptions.Remove(removeTopic);
                }
            }
        }
    }
}