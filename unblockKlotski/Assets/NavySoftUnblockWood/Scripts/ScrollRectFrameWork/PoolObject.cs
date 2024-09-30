﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollRectFrameWork
{
	public class PoolObject : MonoBehaviour
	{
		#region Member Variables

		public bool			isInPool;
		public ObjectPool	pool;
		public CanvasGroup	canvasGroup;

		#endregion
	}
}
