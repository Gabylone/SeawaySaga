using UnityEngine;
using System.Collections;

public class KeepOnLoad : MonoBehaviour {

    public static KeepOnLoad Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
		DontDestroyOnLoad (gameObject);
	}

	public static int dataToLoad = -1;

	public static bool displayTuto = true;

    public string mapName = "";
}
