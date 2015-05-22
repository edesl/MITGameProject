using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]

public class Controller2D : MonoBehaviour {

	public LayerMask collisionMask;

	const float skinWidth = .015f;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	public float maxClimbAngle = 80;
	public float maxDescendAngle = 80;	

	private float horizontalRaySpacing;
	private float verticalRaySpacing;

	// Variables de colisiones
	private BoxCollider2D collider;
	private RayCastOrigins raycastOrigins;

	public CollisionInfo collisions;

	struct RayCastOrigins {

		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	void Start (){

		collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing ();
		
	}

	public void Move (Vector3 velocity){

		UpdateRayCastOrigins ();
		collisions.Reset ();
	
		collisions.velocityOld = velocity;

		if (velocity.y < 0){
			DescendSlope (ref velocity);
		}

		if (velocity.x != 0){ 
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0){
			//Cada cambio que se haga a la variable velocity dentro de VerticalCollisions afectara a la variable velocity de este metodo
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);

	}

	void VerticalCollisions (ref Vector3 velocity){

		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i=0; i<verticalRayCount; i++){
			//Si la direcion de Y es negativa entonces es porque esta colisionando por abajo, de lo contrario, es por arriba
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i+velocity.x);
			RaycastHit2D hit =  Physics2D.Raycast (rayOrigin,Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit){
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				// Esto arregla el problema de que cuando esta escalando y colisiona por arriba con otro objeto, quede vibrando 
				if (collisions.climbingSlope){
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}
				
				// Si colisionamos con algo y vamos hacia abajo, directionY es negativo, si vamos hacia arriba, es positivo
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		//Hace que cuando se va escalando y se cambia de una plataforma a otra con un angulo diferente, no de una especie de salto
		if (collisions.climbingSlope){
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)? raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			// Si hag golpeado y en nuevo slopeAngle no es igual al anterior slopeAngle, entonces es que va a cambiar de obstaculo
			if (hit){
				float slopeAngle = Vector2.Angle (hit.normal,Vector2.up);

				if (slopeAngle != collisions.slopeAngle){
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}

	}

	void HorizontalCollisions (ref Vector3 velocity){
		
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i=0; i<horizontalRayCount; i++){
			//Si la direcion de Y es negativa entonces es porque esta colisionando por abajo, de lo contrario, es por arriba
			Vector2 rayOrigin = (directionX== -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit =  Physics2D.Raycast (rayOrigin,Vector2.right * directionX, rayLength, collisionMask);
			
//			Debug.DrawRay (raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);
			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			
			if (hit){

				//Para conocer el angulo de inclinacion de lo que queremos 
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i==0 && slopeAngle <= maxClimbAngle){
//					Debug.Log ("Angulo: "+slopeAngle);
					float distanceToSlopeStart = 0; 

					if (collisions.descendingSlope){
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}

					if (slopeAngle != collisions.slopeAngleOld){
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				// Cuando se esta escalando no es necesario comprobar las colisiones horizontales
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle){

					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					// Esto arregla el problema de que cuando esta escalando y colisiona por un lado con otro objeto, quede vibrando 
					if (collisions.climbingSlope){
						velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					}

					// Si colisionamos con algo y vamos a la izquierda, directionX es negativo, si vamos a la derecha, es positivo
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;

				}

			}
		}
		
	}

	// Metodo para subir pendientes
	void ClimbSlope (ref Vector3 velocity, float slopeAngle){
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		//Si la velocidad en Y es mayor que la velocidad de subida, se asume que estamos saltando, si no, es porque esta escalando
		if (velocity.y < climbVelocityY){
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;   
		}

	}

	//Metodo para descender pendientes
	void DescendSlope (ref Vector3 velocity){
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1)? raycastOrigins.bottomRight: raycastOrigins.bottomLeft; 
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit){
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle){

				if (Mathf.Sign (hit.normal.x) == directionX){

					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x)){
						float moveDistance = Mathf.Abs (velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void UpdateRayCastOrigins () {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x,bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x,bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x,bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x,bounds.max.y);
	}

	void CalculateRaySpacing (){
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount,2,int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount,2,int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount-1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount-1);
		
	}

	public struct CollisionInfo {
		
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public void Reset (){
			
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
		
	}
}
