using Vuforia;

/* source: https://developer.vuforia.com/library/articles/Solution/How-To-Create-a-Simple-Cloud-Recognition-App-in-Unity */

public class hackNYscript : MonoBehaviour, ICloudRecoEventHandler {
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";
	private string mTargetName = "";
	public ImageTargetBehaviour ImageTargetTemplate;
	ObjReader.ObjData objData;
	public string baseUrl = "http://www.mazinzakaria.com/";
	public string fileName = "";
	public Material standardMaterial;
	string loadingText = "";
	bool loading = false;
	public string name = "";

	void OnGUI () {
		GUILayout.BeginArea (new Rect(10, 10, 400, 400));
		GUILayout.Label (loadingText);
		GUILayout.EndArea();
	}

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

			yield return null;
		}
		fileName = "";
		loading = false;
		if (objData == null || objData.gameObjects == null) {
			loadingText = "Error loading file";
			yield return null;
			yield break;
		}
		
		loadingText = "";
		GameObject userModel;
		userModel = GameObject.Find(name);
		GameObject imgtarget;
		imgtarget = GameObject.Find("ImageTarget");
		
		
		userModel.transform.parent = imgtarget.transform;

	}

	void Start () {
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}
	}

	public void OnInitialized() {
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
		mTargetName = targetSearchResult.TargetName;
		mCloudRecoBehaviour.CloudRecoEnabled = false;
		name = targetSearchResult.TargetName;
		//Debug.LogError (mTargetName);

		if (ImageTargetTemplate) {
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			ImageTargetBehaviour imageTargetBehaviour =
				(ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(
					targetSearchResult, ImageTargetTemplate.gameObject);
		}
		fileName = baseUrl + mTargetName + ".obj";
		if (!loading) {
			StartCoroutine (Load ());
		}

	}


}





