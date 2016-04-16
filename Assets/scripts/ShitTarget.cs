using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShitTarget : MonoBehaviour {

	public static List<ShitTarget> allTargets = new List<ShitTarget>();

	public bool triggered { get { return activators.Count > 0; } }
	bool wasTriggered = false;
	List<Collider> activators = new List<Collider>();

	void Awake () {
		allTargets.Clear();
	}

	// Use this for initialization
	void Start () {
		allTargets.Add( this );
	}
	
	// Update is called once per frame
	void Update () {
		((Behaviour)GetComponent("Halo")).enabled = triggered;
		GetComponent<Renderer>().material.color = triggered ? Color.yellow : Color.white;

		if ( triggered != wasTriggered ) {
			if ( triggered ) {
				// play happy sound
			} else {
				// play sad sound
			}

			wasTriggered = triggered;
		}
	}

	void OnTriggerEnter ( Collider col ) {
		if ( !activators.Contains( col ) ) {
			activators.Add( col );
		}
	}

	void OnTriggerExit ( Collider col ) {
		if ( activators.Contains( col ) ) {
			activators.Remove( col );
		}
	}
}
