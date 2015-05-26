using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	public Transform player;
	public Vector2 margin;
	public Vector2 smoothing;
	public BoxCollider2D Bounds;

	private Vector3 min;
	private Vector3 max;
	private Camera camera2D;

	public bool IsFollowing {get; set;}

	public void Start (){
		camera2D = GetComponent<Camera>();
		min = Bounds.bounds.min;
		max = Bounds.bounds.max;
		IsFollowing = true; 
	}

	public void Update (){
		var x = transform.position.x;
		var y = transform.position.y;

		if (IsFollowing){

			if (Mathf.Abs (x - player.position.x) > margin.x ){
				x = Mathf.Lerp (x, player.position.x, smoothing.x * Time.deltaTime);
			}

			if (Mathf.Abs(y - player.position.y) > margin.y){
				y = Mathf.Lerp (y, player.position.y, smoothing.y * Time.deltaTime);
			}

		}

		var cameraHalfWidth = camera2D.orthographicSize * ((float)Screen.width / Screen.height);
		x = Mathf.Clamp (x, min.x + cameraHalfWidth, max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, min.y + camera2D.orthographicSize, max.y - camera2D.orthographicSize);

		transform.position = new Vector3 (x,y, transform.position.z);

	}

}
