
/*
 * Created on 2022
 *
 * Copyright (c) 2022 dotmobstudio
 * Support : dotmobstudio@gmail.com
 */
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private readonly List<GameObject> _list = new List<GameObject>();

        public void CreateObject(GameObject prefab, int count = 1, bool worldPositionStays = false)
        {
            _prefab = prefab;
            for (var i = 0; i < count; ++i)
            {
                var obj = Object.Instantiate(prefab, null, worldPositionStays);
                obj.gameObject.SetActive(false);
                _list.Add(obj);
            }
        }

        public GameObject Get()
        {
            if (_list.Count > 0)
            {
                var obj = _list[0];
                _list.RemoveAt(0);
                if (obj != null)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            
             return Object.Instantiate(_prefab, null, false);
            
        }

        public void Put(GameObject obj)
        {
            if (obj != null)
            {
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(false);
                _list.Add(obj);
            }
        }

        public bool HaveObjectBase()
        {
            return _prefab != null;
        }
    }
}
