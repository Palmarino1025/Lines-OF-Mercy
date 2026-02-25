using UnityEngine;
using System.Collections;

public class NewPlayerMovement : MonoBehaviour {
	
	
	float accel = 0.0f;
	float accel1 = 0.0f;
	float speedMultiplier = 1.0f;
	float moveSpeed = 0.0f;
	float moveSpeed1 = 0.0f;
	bool movingFB = false;
	bool movingLR = false;
	bool running = false;
	float jumpForce = 235.0f;
	static public int gamePause = 1;
	public Collider Foot;
	bool jumpingAllowed;
	public bool grounded;
	float StickMoveSpeedF = 0.0f;
	float StickMoveSpeedLR = 0.0f;
	float StickRotationSpeedLR = 0.0f;
	public float StickSensitivityHorizontal = 100.0f;
	float jumpDectection = 0.1f;

	
	// Use this for initialization
	void Start () {
		
		Cursor.visible = false;
		
		
	}
	
	
	void FixedUpdate() {
		transform.Translate (Vector3.forward * moveSpeed * speedMultiplier * Time.deltaTime); //movement is constant with refresh
		transform.Translate (Vector3.right * moveSpeed1 * speedMultiplier * Time.deltaTime);
	}
	
	// Update is called once per frame
	void Update () {


		//Raycast for jumping----------------------------------------------------------------------------------------------------
		RaycastHit hit;
		Ray landingRay = new Ray (transform.position, Vector3.down);
		if (Physics.Raycast (landingRay, out hit, jumpDectection)) {
			jumpingAllowed = true;	
			grounded = true;
		} else {
			jumpingAllowed = false;	
			grounded = false;
		}
		Debug.DrawLine(landingRay.origin, hit.point);
		//------------------------------------------------------------------------------------------------------------------------


		StickMoveSpeedF = Input.GetAxis("Vertical");//Joystick
		StickMoveSpeedLR = Input.GetAxis("Horizontal");//Joystick
		StickRotationSpeedLR = Input.GetAxis("HorizontalAim");
		
		// stick rotation
		transform.Rotate (Vector3.up * StickRotationSpeedLR * StickSensitivityHorizontal * Time.deltaTime);




		
		
		//-----------------------------------------------------movement speed and logic---------------------------------------------
		if(running == true){

		}
		else if(running == false){
			if(speedMultiplier >= 0.45f){
				speedMultiplier -= 0.03f;
			}
		}
		

		
		
		
		// end of decceleration-------------------------------------------------------------------------------------------------------------
		//-------------------------------------button press-----------------------------------------------	
		
		
		if(Input.GetButton ("Run")){// not working
			running = true;
		}
		if(Input.GetButtonDown ("Run")){// not working
			running = true;
			speedMultiplier = 1.0f;
		}
		if(Input.GetButtonUp ("Run")){
			running = false;
		}
		
		
		
		if(Input.GetButton ("w")){
			if(accel < 5){
				accel += 0.2f;	
			}
			moveSpeed = accel;
			movingFB = true;
		}
		else if(Input.GetButtonUp("w")){	
			movingFB = false; //refer to decceleration control above---
		}
		
		
		
		
		if(Input.GetButton ("s")){
			if(accel > -5){
				accel -= 0.2f;	
			}
			moveSpeed = accel;
			movingFB = true;
		}
		else if(Input.GetButtonUp("s")){
			movingFB = false; //refer to deccel above
		}
		
		
		
		
		
		if(Input.GetButton ("d")){
			if(accel1 < 5){
				accel1 += 0.2f;	
			}
			moveSpeed1 = accel1;
			movingLR = true;
		}
		else if(Input.GetButtonUp("d")){	
			movingLR = false; //refer to decceleration control above---
		}
		
		
		
		
		if(Input.GetButton ("a")){
			if(accel1 > -5){
				accel1 -= 0.2f;	
			}
			moveSpeed1 = accel1;
			movingLR = true;
		}
		else if(Input.GetButtonUp("a")){
			movingLR = false; //refer to deccel above
		}	
		
		
		
		
		
		//Joystick Movement. Looks like a clone of key press. But allows speed based on joystick intensity.
		
		//X
		if(StickMoveSpeedF > 0.0f){
			moveSpeed = 5 * StickMoveSpeedF;
			movingFB = true;
		}
		else if (StickMoveSpeedF < 0.0f){
			moveSpeed = 5 * StickMoveSpeedF;
			movingFB = true;
		}
		else if (StickMoveSpeedF < 0.5f && StickMoveSpeedF > -0.5f){
			movingFB = false;
			moveSpeed = 0.0f;
		}
		
		
		//Z
		if(StickMoveSpeedLR > 0.0f){
			moveSpeed1 = 5 * StickMoveSpeedLR;
			movingLR = true;
		}
		else if (StickMoveSpeedLR < 0.0f){
			moveSpeed1 = 5 * StickMoveSpeedLR;
			movingLR = true;
		}
		else if (StickMoveSpeedLR < 0.5f && StickMoveSpeedLR > -0.5f){
			movingLR = false;
			moveSpeed1 = 0.0f;
			
		}
		
		
		
		
		//	if(grounded == true){
		//		jumpingAllowed = true;
		//	}
		//	else if(grounded == false){
		//		jumpingAllowed = false;
		//	}
		
		//Debug.Log(jumpingAllowed);
		
		
		
		if(Input.GetButtonDown ("Jump") && jumpingAllowed == true){
			transform.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
		}
		
		
		
		//----------------------------------------------------other controls---------------------------------
		

		
		
		
	}
	//replace with raytrace to ground
//	void OnCollisionStay(Collision ground){
//		jumpingAllowed = true;
//		grounded = true;
//	}
//	void OnCollisionExit(Collision ground){
//		jumpingAllowed = false;	
//		grounded = false;
//	}
	
}
