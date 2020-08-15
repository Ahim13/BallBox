using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffect : MonoBehaviour
{
	[SerializeField] private float _speed = 1;
	[SerializeField] private float _effectSize = 0.5f;

	private Coroutine _effect;

	public void ActivateContactEffect(Vector3 point, float effectSize = 0.5f)
	{
		_effectSize = effectSize;
		if(_effect != null)
			StopCoroutine(_effect);
		_effect = StartCoroutine(Contacteffect(point));
	}

	private IEnumerator Contacteffect(Vector3 point)
	{
		var mat = GetComponent<Renderer>().material;
		mat.SetVector("_Center", point);
		float size = _effectSize;
		_speed = size * 2;
		mat.SetFloat("_SpehereRadius", size);
		while (size > -.5f)
		{
			size -= Time.deltaTime * _speed;
			mat.SetFloat("_SpehereRadius", size);
			yield return 0;
		}

		_effect = null;
	}
}