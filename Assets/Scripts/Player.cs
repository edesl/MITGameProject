using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour {

	public float jumpHeight = 5;
	public float timeToJumpMax = .5f;
	public float movSpeed = 6f;

	private Controller2D controller;
	private Vector3 velocity;
	private float gravity;
	private float jumpVelocity;
	private float velocityXSmoothing;
	private float accelerationTimeAirborne = .2f;
	private float accelerationTimeGrounded = .1f;

	void Start (){

		controller = GetComponent <Controller2D>();

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpMax, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpMax;
		Debug.Log ("Gravity: "+gravity + " Jump Velocity: "+jumpVelocity);
	}

	void Update (){

		// Si esta colisionando por arriba o por abajo, la velocidad en y es igual a 0, para evitar que siga mandando raycast
		if (controller.collisions.above || controller.collisions.below){
			velocity.y = 0;
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"),Input.GetAxisRaw ("Vertical"));

		//Salta si está tocando el piso, es decir, colisionando por abajo
		if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below){
			velocity.y = jumpVelocity;
		}

		//Hace que la velodidad en X varie de acuerdo a si el personaje esta en el suelo o en el aire
		float targetVelocityX = input.x * movSpeed;
		//El tiempo para hacer el SmoothDamp dependera de los tiempos accelerationTimeAirborne y accelerationTimeGrounded
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		//Aplico la gravedad a la velocidad
		velocity.y += gravity * Time.deltaTime;
		//Metodo para mover al personaje
		controller.Move (velocity * Time.deltaTime);
	}
	
}
