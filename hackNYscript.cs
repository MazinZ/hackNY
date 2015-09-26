using UnityEngine;
using System.Collections;
using Vuforia;

/* source: https://developer.vuforia.com/library/articles/Solution/How-To-Create-a-Simple-Cloud-Recognition-App-in-Unity */

public class hackNYscript : MonoBehaviour, ICloudRecoEventHandler {
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";


	void Start () {
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}
	}

	public void OnInitialized() {
		Debug.Log ("Cloud Reco initialized");
	}

	public void OnInitError(TargetFinder.InitState initError) {
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}
	public void OnUpdateError(TargetFinder.UpdateState updateError) {
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
	}
	
	public void OnStateChanged(bool scanning) {
		mIsScanning = scanning;
		if (scanning)
		{
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);
		}
	}


	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		mTargetMetadata = targetSearchResult.MetaData;
		mCloudRecoBehaviour.CloudRecoEnabled = false;
		Debug.LogError (mTargetMetadata);
	}


}





