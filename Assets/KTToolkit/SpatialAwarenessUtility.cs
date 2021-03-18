using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;

namespace KTToolkit.SpacialAwareness
{
    public class SpatialAwarenessUtility : MonoBehaviour
    {
        private IMixedRealitySpatialAwarenessMeshObserver observer;
        public IMixedRealitySpatialAwarenessMeshObserver Observer
        {
            get => this.observer;
            private set => this.observer = value;
        }

        [SerializeField] private UnityEvent loadCallback;
        private void Awake()
        {
            this.Initialize();
            loadCallback?.Invoke();
        }
        private void OnEnable()
        {
            this.Initialize();
        }
        private void Initialize()
        {
            // Use CoreServices to quickly get access to the IMixedRealitySpatialAwarenessSystem
            var spatialAwarenessService = CoreServices.SpatialAwarenessSystem;
            // Cast to the IMixedRealityDataProviderAccess to get access to the data providers
            var dataProviderAccess = spatialAwarenessService as IMixedRealityDataProviderAccess;
#if !UNITY_WSA || UNITY_EDITOR
            var meshObserverName = "Spatial Object Mesh Observer";
            observer = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);
#else
            var meshObserverName = "Windows Mixed Reality Spatial Mesh Observer";
            observer = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);
#endif
        }
        public void EnableSpatial()
        {
            observer.Resume();
        }
        public void DisableSpatial()
        {
            observer.Suspend();
        }
        public void SetVisible()
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
        }
        public void SetOcclusion()
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
        }
        public void SetNone()
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
        }
    }
}
