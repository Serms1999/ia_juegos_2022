using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * @class Agent
 * @brief Representación de un agente.
 */
[AddComponentMenu("Steering/InteractiveObject/Agent")]
public class Agent : Bodi
{

	private const float DefaultLineLength = 5f;

	[Tooltip("Radio interior de la IA")] [SerializeField]
	protected float _interiorRadius = 1f;

	[Tooltip("Radio de llegada de la IA")] [SerializeField]
	protected float _arrivalRadius = 3f;

	[Tooltip("Ángulo interior de la IA")] [SerializeField]
	protected float _interiorAngle = 3.0f; // ángulo sexagesimal.

	[Tooltip("Ángulo exterior de la IA")] [SerializeField]
	protected float _exteriorAngle = 8.0f; // ángulo sexagesimal.

	[SerializeField]
	private float lookAhead = 3f;
	
	[SerializeField] [Range(1, 25)] 
	private int numRays = 3;
	
	[SerializeField] 
	private float raySeparation = 1f;
	
	private List<Vector3> _rays = new List<Vector3>();

	[Header("Depuración")]
	[SerializeField] protected bool drawRadius; // dibujar radios

	[SerializeField] protected bool drawVelocity; // dibujar velocidad y aceleracion

	[SerializeField] protected bool drawAngles; // dibujar angulos
	
	[SerializeField] protected bool drawRays; // dibujar los bigotes del agente

	[Header("Equipo")] 
	[SerializeField] protected Teams team;	// Equipo del agente

	protected List<Node> path;	// Camino que sigue el agente
	
	protected bool dead = false; // Agente vivo
	
	protected Dictionary<TerrainType, float> terrainCosts;	// Costes por terreno del agente

	/**
	 * @return Camino que sigue el agente.
	 */
	public List<Node> Path
	{
		get { return path; }
		set { path = value; }
	}
	
	/**
     * @return Devuelve si el personaje está muerto.
     */
	public bool Dead
	{
		get { return dead; }
	}



	// AÑADIR LAS PROPIEDADES PARA ESTOS ATRIBUTOS. SI LO VES NECESARIO.
	/**
	 * @return Radio interior del agente.
	 */
	public float InteriorRadius
	{
		get { return _interiorRadius; }
		set { _interiorRadius = value; }
	}
	
	/**
	 * @return Radio exterior del agente.
	 */
	public float ArrivalRadius
	{
		get { return _arrivalRadius; }
		set { _arrivalRadius = value; }
	}

	/**
	 * @return Ángulo interior del agente.
	 */
	public float InteriorAngle
	{
		get { return _interiorAngle; }
		set { _interiorAngle = value; }
	}
	
	/**
	 * @return Ángulo exterior del agente.
	 */
	public float ExteriorAngle
	{
		get { return _exteriorAngle; }
		set { _exteriorAngle = value; }
	}
	
	/**
	 * @return Lista de bigotes del agente.
	 */
	public List<Vector3> Rays
	{
		get { return new List<Vector3>(_rays); }
	}

	/**
	 * @return Equipo del agente.
	 */
	public Teams Team
	{
		get { return team; }
		set { team = value; }
	}
		
	/**
	 * @brief Devuelve el coste del terreno.
	 * @param[in] terrain Terreno.
	 * @return Coste del terreno para el agente.
	 */
	public float GetTerrainCost(TerrainType terrain)
	{
		return terrainCosts[terrain];
	}

	protected void Update()
	{
		// Creamos los bigotes del agente
		CreateRays();
	}
	
	private void CreateRays()
	{
		_rays = new List<Vector3>();
		switch (numRays % 2)
		{
			case 0:
			{
				for (int i = -numRays / 2; i <= numRays / 2; i++)
				{
					if (i == 0)
					{
						continue;
					}

					_rays.Add(this.OrientationToVector(_orientation + i * raySeparation) * lookAhead);
				}

				break;
			}
			case 1:
			{
				for (int i = -(numRays - 1) / 2; i <= (numRays - 1) / 2; i++)
				{
					if (i == 0)
					{
						continue;
					}

					_rays.Add(this.OrientationToVector(_orientation + i * raySeparation) * (lookAhead * 0.8f));
				}

				_rays.Insert((numRays - 1) / 2, OrientationToVector() * lookAhead);
				break;
			}
		}
	}

	// AÑADIR LO NECESARIO PARA MOSTRAR LA DEPURACIÓN. Te puede interesar los siguientes enlaces.
	// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
	// https://docs.unity3d.com/ScriptReference/Debug.DrawLine.html
	// https://docs.unity3d.com/ScriptReference/Gizmos.DrawWireSphere.html
	// https://docs.unity3d.com/ScriptReference/Gizmos-color.html

	private void OnDrawGizmos()
	{
		if (drawAngles)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(Position,
				Position + OrientationToVector(Orientation + _interiorAngle) * DefaultLineLength);
			Gizmos.DrawLine(Position,
				Position + OrientationToVector(Orientation - _interiorAngle) * DefaultLineLength);
			
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(Position,
				Position + OrientationToVector(Orientation + _exteriorAngle) * DefaultLineLength);
			Gizmos.DrawLine(Position,
				Position + OrientationToVector(Orientation - _exteriorAngle) * DefaultLineLength);
		}

		if (drawVelocity)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Position, Position + Velocity);
		}

		if (drawRadius)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(Position, _interiorRadius);
			
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(Position, _arrivalRadius);
		}

		if (drawRays && _rays != null)
		{
			Gizmos.color = Color.blue;
			foreach (Vector3 ray in _rays)
			{
				Gizmos.DrawLine(Position, Position + ray);
			}
		}
	}
}
