using System;
using UnityEngine;

public class Bodi : MonoBehaviour
{

	[SerializeField] protected float _mass = 1;
	[SerializeField] protected float _maxSpeed = 1;
	[SerializeField] protected float _maxRotation = 1;
	[SerializeField] protected float _maxAcceleration = 1;
	[SerializeField] protected float _maxAngularAcc = 1;
	[SerializeField] protected float _maxForce = 1;

	
	protected Vector3 _acceleration; // aceleración lineal
	protected float _angularAcc; // aceleración angular
	private Vector3 _velocity; // velocidad lineal
	protected float _rotation; // velocidad angular
	protected float _speed; // velocidad escalar
	protected float _orientation; // 'posición' angular




	/// <summary>
	/// Mass for the NPC
	/// </summary>
	public float Mass
	{
		get { return _mass; }
		set { _mass = value; }
	}

	// CONSTRUYE LAS PROPIEDADES SIGUENTES
	// PUEDES CAMBIAR LOS NOMBRE A TU GUSTO
	// public float MaxForce
	public float MaxForce
	{
		get { return _maxForce; }
		set { _maxForce = value; }
	}

	// public float MaxSpeed
	public float MaxSpeed
	{
		get { return _maxSpeed; }
		set { _maxSpeed = value; }
	}

	// public Vector3 Velocity
	public Vector3 Velocity
	{
		get { return _velocity; }
		set { _velocity = value; }
	}

	// public float MaxRotation
	public float MaxRotation
	{
		get { return _maxRotation; }
		set { _maxRotation = value; }
	}

	// public float Rotation. 
	public float Rotation
	{
		get { return _rotation; }
		set { _rotation = value; }
	}

	// public float MaxAcceleration
	public float MaxAcceleration
	{
		get { return _maxAcceleration; }
		set { _maxAcceleration = value; }
	}

	// public Vector3 Acceleration
	public Vector3 Acceleration
	{
		get { return _acceleration; }
		set { _acceleration = value; }
	}

	// public float AngularAcc
	public float AngularAcc
	{
		get { return _angularAcc; }
		set { _angularAcc = value; }
	}

	// public Vector3 Position. Recuerda. Esta es la única propiedad que trabaja sobre transform.
	public Vector3 Position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	// public float Orientation
	public float Orientation
	{
		get { return _orientation; }
		set { _orientation = value; }
	}

	// public float Speed
	public float Speed
	{
		get { return _speed; }
		set { _speed = value; }
	}

	// TE PUEDEN INTERESAR LOS SIGUIENTES MÉTODOS.
	// Quita o añade todos los que sean referentes a la parte física.

	// public float Heading()
	// public static float MapToRange(float rotation, Range r)
	public static float MapToRange(float rotation, Range r)
	{
		float interval = r.Length;
		float mid = r.MidPoint;
		
		rotation %= interval;
		if (mid == 0f & Mathf.Abs(rotation) > r.End) {
			if (rotation < 0.0f)
				rotation += interval;
			else
				rotation -= interval;
		}
		return rotation;
	}

	// public float MapToRange(Range r)
	public float MapToRange(Range r)
	{
		return MapToRange(_rotation, r);
	}

	// public float PositionToAngle()
	public float PositionToAngle()
	{
		return Mathf.Atan2(Position.x, Position.z) * Mathf.Rad2Deg;
	}
	
	// public float PositionToAngle(Vector 3 position)
	public static float PositionToAngle(Vector3 position)
	{
		return Mathf.Atan2(position.x, position.z) * Mathf.Rad2Deg;
	}

	// public Vector3 OrientationToVector()
	public Vector3 OrientationToVector()
	{
		return OrientationToVector(_orientation);
	}
	
	// public Vector3 OrientationToVector(float orientation)
	public Vector3 OrientationToVector(float orientation)
	{
		float orientationRad = orientation * Mathf.Deg2Rad;
		return new Vector3(Mathf.Sin(orientationRad), 0f, Mathf.Cos(orientationRad));

	}

	// public Vector3 VectorHeading()
	// public float GetMinimumAngleTo(Vector3 rotation)
	public float GetMinimumAngleTo(Vector3 rotation)
	{
		float rotationAngle = Mathf.Atan2(rotation.x, rotation.z) * Mathf.Rad2Deg;
		return MapToRange(rotationAngle, new Range(-180, 180));
	}

	// public void ResetOrientation()
	public void ResetOrientation()
	{
		_orientation = 0f;
	}
	// public float PredictNearestApproachTime(Bodi other, float timeInit, float timeEnd)
	// public float PredictNearestApproachDistance3(Bodi other, float timeInit, float timeEnd)

	public void ResetMovement()
	{
		_velocity = Vector3.zero;
		_rotation = 0f;
	}

}
