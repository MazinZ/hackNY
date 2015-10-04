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

	public string fileName = "https://s3.amazonaws.com/hackny.springhurst/";
	public Material standardMaterial;	
	public ObjReader.ObjData objData;
	string loadingText = "";
	bool loading = false;

	public string materialName;
	
	IEnumerator Load () {
		if (name [0] == 'm') {
			ObjReader.use.scaleFactor = new Vector3 (40F, 40F, 40F);
			ObjReader.use.objRotation = new Vector3 (-70F, 0F, 0F);
			Debug.Log("MATH DETECTED SHRINKING");

		}
		else if (name[0]=='c'){
			ObjReader.use.scaleFactor = new Vector3 (.9F, .9F, .9F);
			Debug.Log("CHEMISTRY DETECTED");
		}
		else {
			ObjReader.use.scaleFactor = new Vector3 (.09F, .09F, .09F);

		}
		loading = true;
		if (objData != null && objData.gameObjects != null) {
			for (var i = 0; i < objData.gameObjects.Length; i++) {
				Destroy (objData.gameObjects[i]);
			}
		}


	
		materialName = fileName + ".mtl";
		//Debug.LogError (materialName);

		objData = ObjReader.use.ConvertFileAsync (fileName, true, standardMaterial );
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

		
		loadingText = "";


		GameObject userModel ;
		userModel = GameObject.Find(name);
		GameObject imgtarget;
		imgtarget = GameObject.Find("ImageTarget");
		//Debug.LogError(userModel.GetComponent<Renderer>().material);
		//userModel.AddComponent<BoxCollider>();
		//Debug.LogError(userModel.GetComponent<BoxCollider>().size.magnitude);

		userModel.transform.parent = imgtarget.transform;

		//imgtarget.transform.localScale += new Vector3 (80f, 80f, 80f);
		//userModel.transform.localScale += new Vector3 (70f, 70f, 70f);

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
		Debug.Log (targetSearchResult.TargetName);
		name = targetSearchResult.TargetName;
		fileName = fileName + targetSearchResult.TargetName.ToString() + ".obj";
		StartCoroutine (Load());


		fileName = "https://s3.amazonaws.com/hackny.springhurst/";
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
			if (GUI.Button(new Rect(Screen.width / 10F,Screen.height / 10F,(Screen.width - (Screen.width / 10F)),(Screen.height / 10F)), "New Scan")) {
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}
	}
	



}











