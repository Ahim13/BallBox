using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Side
{
	Bottom,
	Left,
	Right,
	Top,
	Front,
	Back
}

public class SpherePhysics : MonoBehaviour
{
	[SerializeField]
	private GameObject Box;
	[SerializeField]
	private CollisionEffect CollisionEffect;

	[SerializeField]
	private Vector3 F = Vector3.one;
	[SerializeField]
	private Vector3 a = Vector3.one;
	[SerializeField]
	private Vector3 v = Vector3.one;
	[SerializeField]
	private float m = 1;

	private Plane _planeTop;
	private Plane _planeBot;
	private Plane _planeRight;
	private Plane _planeLeft;
	private Plane _planeBack;
	private Plane _planeFront;
	
	private float _dist;
	private float _radius;
	private float _distance;

	private Vector3 _extents;
	private Vector3 _up;
	private Vector3 _right;
	private Vector3 _forward;
	private Vector3 _position;
	private Vector3 _closestPoint;

	private Vector3 _normalTop;
	private Vector3 _normalBot;
	private Vector3 _normalLeft;
	private Vector3 _normalRight;
	private Vector3 _normalFront;
	private Vector3 _normalBack;
	
	private void Start()
	{
		SetupValues();

		_planeTop = new Plane((_position - (_position + _up)).normalized, _position + (_up * _extents.y));
		_planeBot = new Plane((_position - (_position - _up)).normalized, _position - (_up * _extents.y));
		_planeRight = new Plane((_position - (_position + _right)).normalized, _position + (_right * _extents.x));
		_planeLeft = new Plane((_position - (_position - _right)).normalized, _position - (_right * _extents.x));
		_planeBack = new Plane((_position - (_position + _forward)).normalized, _position + (_forward * _extents.z));
		_planeFront = new Plane((_position - (_position - _forward)).normalized, _position - (_forward * _extents.z));
	}
	
	private void FixedUpdate()
	{
		a = F / m;
		v = v + a * Time.deltaTime;
		transform.position += v * Time.deltaTime + a * (Time.deltaTime * Time.deltaTime) / 2;
		F = Vector3.zero;
	}

	private void Update()
	{
		SetupValues();

		_normalTop = (_position - (_position + _up)).normalized;
		_normalBot = (_position - (_position - _up)).normalized;
		_normalRight = (_position - (_position + _right)).normalized;
		_normalLeft = (_position - (_position - _right)).normalized;
		_normalBack = (_position - (_position + _forward)).normalized;
		_normalFront = (_position - (_position - _forward)).normalized;
		
		_planeTop.SetNormalAndPosition(_normalTop, (_position + (_up * _extents.y)));
		_planeBot.SetNormalAndPosition(_normalBot, (_position - (_up * _extents.y)));
		_planeRight.SetNormalAndPosition(_normalRight, (_position + (_right * _extents.x)));
		_planeLeft.SetNormalAndPosition(_normalLeft, (_position - (_right * _extents.x)));
		_planeBack.SetNormalAndPosition(_normalBack, (_position + (_forward * _extents.z)));
		_planeFront.SetNormalAndPosition(_normalFront, (_position - (_forward * _extents.z)));

		#region Intersection

		if (IsIntersectsWithSide(Side.Top, out _closestPoint, out _distance))
		{
			Collided(_normalTop, _closestPoint, _distance);
		}

		if (IsIntersectsWithSide(Side.Bottom, out _closestPoint, out _distance))
		{
			Collided(_normalBot, _closestPoint, _distance);
		}

		if (IsIntersectsWithSide(Side.Right, out _closestPoint, out _distance))
		{
			Collided(_normalRight, _closestPoint, _distance);
		}

		if (IsIntersectsWithSide(Side.Left, out _closestPoint, out _distance))
		{
			Collided(_normalLeft, _closestPoint, _distance);
		}

		if (IsIntersectsWithSide(Side.Back, out _closestPoint, out _distance))
		{
			Collided(_normalBack, _closestPoint, _distance);
		}

		if (IsIntersectsWithSide(Side.Front, out _closestPoint, out _distance))
		{
			Collided(_normalFront, _closestPoint, _distance);
		}

		#endregion
	}

	private void SetupValues()
	{
		_extents = Box.transform.localScale / 2;
		_radius = transform.localScale.x / 2;
		_up = Box.transform.up;
		_right = Box.transform.right;
		_forward = Box.transform.forward;
		_position = Box.transform.position;
	}

	private void Collided(Vector3 normal, Vector3 closestPoint, float distance)
	{
		CollisionEffect.ActivateContactEffect(closestPoint, transform.localScale.x / 2.0f);
		var position = transform.position;
		if (Vector3.Dot(_closestPoint - position, v) > 0)
		{
			var newDirection = Vector3.Reflect(v, normal);
			v = newDirection;
		}

		var shift = _radius - distance;
		position += (position - _closestPoint).normalized * shift;
		transform.position = position;
	}


	private bool IsIntersectsWithSide(Side side, out Vector3 closestPoint, out float distance)
	{
		switch (side)
		{
			case Side.Bottom:
				closestPoint = _planeBot.ClosestPointOnPlane(transform.position);
				break;
			case Side.Top:
				closestPoint = _planeTop.ClosestPointOnPlane(transform.position);
				break;
			case Side.Left:
				closestPoint = _planeLeft.ClosestPointOnPlane(transform.position);
				break;
			case Side.Right:
				closestPoint = _planeRight.ClosestPointOnPlane(transform.position);
				break;
			case Side.Front:
				closestPoint = _planeFront.ClosestPointOnPlane(transform.position);
				break;
			case Side.Back:
				closestPoint = _planeBack.ClosestPointOnPlane(transform.position);
				break;
			default:
				closestPoint = new Vector3(-1, -1, -1);
				distance = -1;
				return false;
		}

		distance = Mathf.Sqrt((closestPoint.x - transform.position.x) * (closestPoint.x - transform.position.x) +
		                          (closestPoint.y - transform.position.y) * (closestPoint.y - transform.position.y) +
		                          (closestPoint.z - transform.position.z) * (closestPoint.z - transform.position.z));

		return distance < _radius;
	}

	public void SetSphereRadius(float x)
	{
		transform.localScale = new Vector3(x, x, x);;
	}

	public void PushSphere()
	{
		F = new Vector3(150,150,150);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, v.normalized * GetComponent<Renderer>().bounds.extents.x);
		Gizmos.color = Color.magenta;

		Gizmos.DrawRay(_position + (_up * _extents.y), _planeTop.normal);
		Gizmos.DrawRay(_position - (_up * _extents.y), _planeBot.normal);
		Gizmos.DrawRay(_position + (_right * _extents.x), _planeRight.normal);
		Gizmos.DrawRay(_position - (_right * _extents.x), _planeLeft.normal);
		Gizmos.DrawRay(_position + (_forward * _extents.z), _planeBack.normal);
		Gizmos.DrawRay(_position - (_forward * _extents.z), _planeFront.normal);
	}
}