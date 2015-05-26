using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
	public class DeadZone : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Player" || other.tag == "Platform")
			{
				Application.LoadLevel(Application.loadedLevelName);
			}
		}
	}
}
