using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMovieBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MovieTexture mt = (MovieTexture)GetComponent<RawImage>().mainTexture;
        mt.loop = true;
        mt.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
