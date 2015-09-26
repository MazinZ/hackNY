using UnityEngine;
using System.Collections;
using Vuforia;

/* source: https://developer.vuforia.com/library/articles/Solution/How-To-Create-a-Simple-Cloud-Recognition-App-in-Unity */


public class hackNYscript : MonoBehaviour, ICloudRecoEventHandler {
	public ImageTargetBehaviour ImageTargetTemplate;
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";
	public string name;

	public string fileName = "http://www.mazinzakaria.com/";
	public Material standardMaterial;	
	ObjReader.ObjData objData;
	string loadingText = "";
	bool loading = false;
	
	IEnumerator Load () {
		loading = true;
		if (objData != null && objData.gameObjects != null) {
			for (var i = 0; i < objData.gameObjects.Length; i++) {
				Destroy (objData.gameObjects[i]);
			}
		}
		objData = ObjReader.use.ConvertFileAsync (fileName, true, standardMaterial);
		while (!objData.isDone) {
			loadingText = "Loading... " + (objData.progress*100).ToString("f0") + "%";
			if (Input.GetKeyDown (KeyCode.Escape)) {
				objData.Cancel();
				loadingText = "Cancelled download";
				loading = false;
				yield break;
			}
			yield return null;
		}
		loading = false;

		
		loadingText = "Import completed";
		
		GameObject hand;
		hand = GameObject.Find(name);
		GameObject imgtarget;
		imgtarget = GameObject.Find("ImageTarget");
		
		
		hand.transform.parent = imgtarget.transform;

	}

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
		Debug.LogError (targetSearchResult.TargetName);
		name = targetSearchResult.TargetName;
		fileName = fileName + targetSearchResult.TargetName.ToString() + ".obj";
		StartCoroutine (Load());


		fileName = "http://www.mazinzakaria.com/";
		mCloudRecoBehaviour.CloudRecoEnabled = false;

		if (ImageTargetTemplate) {
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			ImageTargetBehaviour imageTargetBehaviour =
				(ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(
					targetSearchResult, ImageTargetTemplate.gameObject);
		}
	}
	
	void OnGUI() {
		GUILayout.Label (loadingText);
		if (!mIsScanning) {
			if (GUI.Button(new Rect(100,300,200,50), "Restart Scanning")) {
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}
	}


}







