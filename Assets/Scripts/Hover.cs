using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public float ForceNorm = 100f;						//force that comes out of each jet, no matter what
	public float ForceBalance = 500f;					//force multiplier for each jet when stablising
	public float DefaultDesiredDist = 2f;			//the default height off the ground when stablised
	public float ForceForward = 500f;					//accelleration force
	public float TurnAmpFront = 1.7f;				//when turning, the height of the front outside wheel is multiplied by this
	public float TurnAmpBack = 1.0f;				//as above, but back wheel
	public float FrontTurnForce = 0f;				//ignore
	public float DistPower = 5f;						// ((desired dist - actual dist) / desired dist) is raised to this power to make it less bumpy

	Rigidbody rigidBody;

	Transform[,] JetLocations;
	Transform Back;
	Transform Front;
	float [,] DesiredDist;

	void Start () {
		rigidBody = this.GetComponent<Rigidbody>();
		JetLocations = new Transform[2,2];
		DesiredDist = new float[2,2];
		DesiredDist[0,0] = DesiredDist[0,1] = DesiredDist[1,0] = DesiredDist[1,1] = DefaultDesiredDist;		//[0,0] = front left, [0,1] = front right, [1,0] = back left, [1,1] = back right

		var transforms = this.GetComponentsInChildren<Transform>();

		foreach (var t in transforms){
			switch(t.name){
			case "JetFrontLeft":
				JetLocations[0,0] = t;
				break;
			case "JetFrontRight":
				JetLocations[0,1] = t;
				break;
			case "JetBackLeft":
				JetLocations[1,0] = t;
				break;
			case "JetBackRight":
				JetLocations[1,1] = t;
				break;
			case "Front":
				Front = t;
				break;
			case "Back":
				Back = t;
				break;
			}
		}
	}
	
	void Update () {
		float[,] currentForce = new float[2,2];

		if(Input.GetAxis("Vertical") > 0.5f){
			rigidBody.AddForceAtPosition(rigidBody.rotation * Vector3.right * ForceForward, rigidBody.position);
		}

		var horizontal = Input.GetAxis("Horizontal");
		if(horizontal < -0.5){
			//Turning left...
			DesiredDist[0,1] = DefaultDesiredDist * TurnAmpFront;
			DesiredDist[1,1] = DefaultDesiredDist * TurnAmpBack;
			//currentForce[0,1] = FrontTurnForce;
		}
		if(horizontal > 0.5){
			//Turning right...
			DesiredDist[0,0] = DefaultDesiredDist * TurnAmpFront;
			DesiredDist[1,0] = DefaultDesiredDist * TurnAmpBack;
			//currentForce[0,0] = FrontTurnForce;
		}

		//JetForce(JetLocations[0,1], currentForce[0,1]);
		//JetForce(JetLocations[0,0], currentForce[0,0]);

		ApplyStabilizeForce(JetLocations[0,0], DesiredDist[0,0]);
		ApplyStabilizeForce(JetLocations[0,1], DesiredDist[0,1]);
		ApplyStabilizeForce(JetLocations[1,0], DesiredDist[1,0]);
		ApplyStabilizeForce(JetLocations[1,1], DesiredDist[1,1]);

		DesiredDist[0,0] = DesiredDist[0,1] = DesiredDist[1,0] = DesiredDist[1,1] = DefaultDesiredDist;
	}

	private void JetForce (Transform pos, float force){
		rigidBody.AddForceAtPosition(pos.rotation * Vector3.up * force, pos.position);
	}

	private void ApplyStabilizeForce (Transform pos, float desiredDist){
		var height = GetHeight (pos);
		var heightDiff = desiredDist - height;
		var jetRatio = heightDiff / desiredDist;
		jetRatio = Mathf.Clamp(jetRatio, 0f, 1f);

		JetForce (pos, ForceBalance * Mathf.Pow(jetRatio, DistPower));
		JetForce (pos, ForceNorm);
	}

	private float GetHeight (Transform pos){
		RaycastHit hit;
		if(Physics.Raycast(pos.position, pos.rotation * Vector3.down, out hit)){
			return hit.distance;
		}
		return float.MaxValue;
	}
}








