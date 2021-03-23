using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Helper.InputHelper;
using Slime.Helper;
using Slime.Helper.Context;
using Slime.Helper.DI;
using Slime.Helper.GameHelper;
using Slime.Helper.Geometry;
using Slime.Helper.InputHelper;
using Slime.Helper.IO;
using Slime.Helper.Scene;
using UnityEngine;

namespace Helper.Context
{
    public class MainContext : MonoBehaviour
    {
        public static MainContext Instance { get; private set; }
        [SerializeField] private GameObject [] _monoBehaviours;   
        private ConcurrentQueue<Action> _invokeQueue = new ConcurrentQueue<Action>();
        private List<IManualUpdate> _manualUpdates = new List<IManualUpdate>();

        private Camera _camera;
        // Start is called before the first frame update
        void Awake()
        {
            Util.DontDestroyOnLoad<MainContext>(gameObject);
            Instance = this;
            CreateMonoBehaviours();
            _camera = Camera.main;
            RegisterContextElements();
            //initialize camera and its borders after DIContainer is initialized
            Util.ThreadStart(() =>
            {
                Thread.Sleep(500);
                Invoke(() =>
                {
                    FlowManager.LoadScene(FlowManager.GameIndex);
                });
            });
        }

        void RegisterContextElements()
        {
            var coordinateHelper = DIContainer.RegisterSingle<CoordinateHelper>(new Type[] {typeof(Camera)},new object[] {_camera});
            var touchHelper = DIContainer.RegisterSingle<TouchHelper>(new Type[] {typeof(CoordinateHelper)},new object[] {coordinateHelper});
            
            DIContainer.RegisterSingle<IOHelper>();
            _manualUpdates.Add(touchHelper);
        }

        void CreateMonoBehaviours()
        {
            foreach (var monoBehaviour in _monoBehaviours)
            {
                Instantiate(monoBehaviour);
            }
        }

        // Update is called once per frame
        void Update()
        {
            while (_invokeQueue.TryDequeue(out var action))
            {
                action();
            }

            foreach (var manualUpdate in _manualUpdates)
            {
                manualUpdate.ManualUpdate();
            }
        }

        public void Invoke(Action action)
        {
            _invokeQueue.Enqueue(action);
        }


        public void Play()
        {
            FlowManager.LoadScene(FlowManager.GameIndex);
        }
        
    }
}
