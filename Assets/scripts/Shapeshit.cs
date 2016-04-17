using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shapeshit : MonoBehaviour {

	public static Shapeshit instance;
	public Transform chunkPrefab;
	List<Transform> chunks = new List<Transform>();
	Transform lastChunk { get { return chunks[chunks.Count-1]; } }

	float speed = 1.5f;
	float gap = 0.2f;
	float v, h, progress;
	Quaternion startRot;
	float chunkScale = 1.35f;

	bool canPoop;
	public bool isPooping = false;
	public GameObject victory;

	// Use this for initialization
	void Start () {
		instance = this;
		startRot = transform.rotation;

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButton(0) ) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) ) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		h = Mathf.Clamp( h + Input.GetAxis("Mouse X") * Time.deltaTime * 400f, -60f, 60f);
		v = Mathf.Clamp( v - Input.GetAxis("Mouse Y") * Time.deltaTime * 1000f, -40f, 40f);
		transform.rotation = Quaternion.Slerp( transform.rotation, startRot * Quaternion.Euler( v, h, 0f ), Time.deltaTime * 5f);

		if ( Input.GetMouseButtonDown(0) ) {
			canPoop = true;
			speed = Random.Range(1f, 3f);
		}
			
		if ( Input.GetMouseButton(0) && canPoop ) {
			isPooping = true;
			progress += Time.deltaTime;
			if ( progress >= gap / speed * chunkScale ) {
				var newChunk = (Transform)Instantiate( chunkPrefab, transform.position, transform.rotation * Quaternion.Euler(0f, 0f, Random.Range(0f, 270f) ) );
				newChunk.GetChild(0).localScale = Vector3.one * 0.5f;
				chunks.Add(newChunk);
				progress = 0f;
			}

			// parent chunks forward
			for ( int i=0; i<chunks.Count; i++ ) {
				if ( chunks.Count > 1 && i < chunks.Count-1 && chunks[i].parent == null ) {
					chunks[i].SetParent( chunks[i+1] );
				}
				//c.localPosition += new Vector3(0f, 0f, increment) * Time.deltaTime;
			}

			if ( chunks.Count > 0 ) {
				lastChunk.GetChild(0).localScale = Vector3.Lerp( lastChunk.GetChild(0).localScale, Vector3.one * (chunkScale + Random.Range(1f, 2f) ), Time.deltaTime * 5f );
				lastChunk.Translate(0f, 0f, speed * Time.deltaTime);
			}
		}


		// you have to limit the chunkCount because deeply nested rigidbodies will break Unity
		if ( chunks.Count > 0 && (Input.GetMouseButtonUp(0) || chunks.Count > jointDist * 16) ) {
			Detach();
		}

		// win detection
		bool didIWin = true;
		foreach ( var target in ShitTarget.allTargets ) {
			if ( !target.triggered ) {
				didIWin = false;
				break;
			}
		}

		victory.SetActive( didIWin );

	}

	const int jointDist = 4;
	public void Detach () {
		if ( chunks.Count == 0 || !isPooping )
			return;

		isPooping = false;
		var rb = lastChunk.gameObject.AddComponent<Rigidbody>();
		rb.drag = 0.01f;

		for( int i=chunks.Count-jointDist ; i>jointDist ; i-=jointDist ) {
			rb = Jointify( rb, i );
		}

		rb.AddForce( transform.forward * 500f * chunks.Count);
		chunks.Clear();
		canPoop = false;
	}

	Rigidbody Jointify (Rigidbody rootRB, int atIndex ) {
		var start = chunks[atIndex-1];
		var end = chunks[atIndex];

		//Debug.Log( atIndex-1 );
		var startRB = start.gameObject.AddComponent<Rigidbody>();
		rootRB.mass = 100f;
		//var endRB = end.gameObject.AddComponent<Rigidbody>();

		var joint = start.gameObject.AddComponent<CharacterJoint>();
		joint.connectedBody = rootRB;
		joint.connectedAnchor = rootRB.transform.InverseTransformPoint( end.position );
		var lowLimit = new SoftJointLimit();
		lowLimit.limit = 0f;
		var highLimit = new SoftJointLimit();
		highLimit.limit = 100f;
		var spring = new SoftJointLimitSpring();
		spring.spring = 0.1f;
		spring.damper = 0f;
		joint.lowTwistLimit = highLimit;
		joint.highTwistLimit = highLimit;
		joint.twistLimitSpring = spring;
		joint.swingLimitSpring = spring;
		joint.swing1Limit = highLimit;
		joint.swing2Limit = lowLimit;

		return startRB;
	}
}
