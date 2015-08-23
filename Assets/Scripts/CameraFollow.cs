using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	[Range(0f, 10f)]
	public float MouseSensitivity = 2.0f;
	public GameObject target;

	public float FieldOfViewMagnifier;
	public float FieldOfViewMax;

	public float LerpConstant = 100f;
	private Rigidbody targetRigidBody;

	private Camera camera;

	Vector3 offset;

	void Start () {
		offset = this.transform.position - target.transform.position;
		targetRigidBody = target.GetComponent<Rigidbody>();
		camera = this.GetComponent<Camera>();
	}
	
	void Update () {
		var mouseAng = Quaternion.Euler(Input.GetAxis ("Mouse Y") * MouseSensitivity, Input.GetAxis("Mouse X") * MouseSensitivity, 0);
		offset = mouseAng * offset;
		this.transform.rotation = Quaternion.LookRotation(-offset);

		this.transform.position = Vector3.Lerp (this.transform.position, target.transform.position + offset, LerpConstant);

		var flatVelVect = new Vector2(targetRigidBody.velocity.x, targetRigidBody.velocity.z);

		camera.fieldOfView = Mathf.Clamp(50 + flatVelVect.magnitude * FieldOfViewMagnifier, 0, FieldOfViewMax);
	}
}

