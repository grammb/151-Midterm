using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{

	public GameObject player;
	private Vector3 offset;

	// Use this for initialization
	void Start()
	{
		offset = transform.position - player.transform.position;
	}

	// Update is called once per frame
	//call late update instead of update.
	void LateUpdate()
	{
		transform.position = player.transform.position + offset;
	}
}
