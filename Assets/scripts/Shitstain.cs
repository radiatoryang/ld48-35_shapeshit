using UnityEngine;
using System.Collections;

public class Shitstain : MonoBehaviour {
	//AudioSource audio;
	public ParticleSystem particles;

	ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
	void Start() {
		//emit = new ParticleSystem.EmitParams();
		//audio = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider col) {
		if ( Shapeshit.instance.isPooping ) {
			Shapeshit.instance.Detach();
		}

	//	Debug.Log("stain!");

	//	emit.startSize = 10f;
	//	emit.startLifetime = 100f;
		emit.startSize = Random.Range( 1.5f, 3f) * 2f;
		emit.position = new Vector3( col.transform.position.x, -9.4f, col.transform.position.z );
		particles.Emit( emit, 1 );

		//audio.Play();
	}

}
