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

	public static bool displayTuto = false;

    public Map map;
    public string mapName = "";
    public int price = 666;

    public static int pearls = 0;
}
