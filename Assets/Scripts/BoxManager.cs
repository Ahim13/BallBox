using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoxManager : MonoBehaviour
{
	[SerializeField] private bool _doScale;
	[SerializeField] private bool _doRotate;
	[SerializeField] private bool _doMove;

	[SerializeField] private GameObject _sphere;
	[SerializeField] private List<Vector3> _wayPoints;

	[SerializeField] private float _rotateSpeed = 5;
	[SerializeField] private float _scaleSpeed = 2;
	[SerializeField] private float _scaleTime = 5;
	[SerializeField] private float _moveSpeed = 2;

	[Header("UI")] [SerializeField] private Slider _sliderX;
	[SerializeField] private Slider _sliderY;
	[SerializeField] private Slider _sliderZ;
	
	private Vector3 _randomAngle;
	private int _target;

	private void Start()
	{
		StartCoroutine(Scale());
		_randomAngle = new Vector3(Random.Range(0, 90f), Random.Range(0, 90f), Random.Range(0, 90f));
		
		SetupBoxSize();
	}

	private void Update()
	{
		if (_doRotate)
			transform.Rotate(_randomAngle, _rotateSpeed * Time.deltaTime, Space.Self);
		if (_doMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, _wayPoints[_target], _moveSpeed * Time.deltaTime);
			if (transform.position == _wayPoints[_target])
			{
				_target++;
				if (_target == _wayPoints.Count)
					_target = 0;
			}
		}

		var sphereLocalScale = _sphere.transform.localScale;
		_sliderX.minValue = sphereLocalScale.x;
		_sliderX.maxValue = _sliderX.minValue + 20;
		_sliderY.minValue = sphereLocalScale.x;
		_sliderY.maxValue = _sliderY.minValue + 20;
		_sliderZ.minValue = sphereLocalScale.x;
		_sliderZ.maxValue = _sliderZ.minValue + 20;
	}

	IEnumerator Scale()
	{
		while (Application.isPlaying)
		{
			if (_doScale)
			{
				var time = _scaleTime;
				while (time > 0)
				{
					transform.localScale += Vector3.one * (Time.deltaTime * _scaleSpeed);
					time -= Time.deltaTime;
					yield return null;
				}

				time = _scaleTime;
				while (time > 0)
				{
					var localScale = transform.localScale;
					var ballLocalScale = _sphere.transform.localScale;
					localScale -= Vector3.one * (Time.deltaTime * _scaleSpeed);
					transform.localScale = localScale;
					time -= Time.deltaTime;
					if (localScale.x <= ballLocalScale.x || localScale.y <= ballLocalScale.y || localScale.z <= ballLocalScale.z)
					{
						break;
					}
					yield return null;
				}
			}
			else
				yield return null;
		}
	}
	
	private void SetupBoxSize()
	{
		var sphereLocalScale = _sphere.transform.localScale;
		_sliderX.minValue = sphereLocalScale.x;
		_sliderX.maxValue = _sliderX.minValue + 20;
		_sliderX.value = _sliderX.maxValue / 2f;
		_sliderY.minValue = sphereLocalScale.x;
		_sliderY.maxValue = _sliderY.minValue + 20;
		_sliderY.value = _sliderX.maxValue / 2f;
		_sliderZ.minValue = sphereLocalScale.x;
		_sliderZ.maxValue = _sliderZ.minValue + 20;
		_sliderZ.value = _sliderX.maxValue / 2f;
	}

	public void SetBoxScaleX(float x)
	{
		var localScale = transform.localScale;
		localScale = new Vector3(x, localScale.y, localScale.z);
		transform.localScale = localScale;
	}
	
	public void SetBoxScaleY(float y)
	{
		var localScale = transform.localScale;
		localScale = new Vector3(localScale.x, y, localScale.z);
		transform.localScale = localScale;
	}
	
	public void SetBoxScaleZ(float z)
	{
		var localScale = transform.localScale;
		localScale = new Vector3(localScale.x, localScale.y, z);
		transform.localScale = localScale;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}
}