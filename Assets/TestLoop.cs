using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoop : MonoBehaviour
{

    public int s;

    public GameObject prefab;

    public int a = 1;

    public Coords coords;

    Dictionary<Coords, SpriteRenderer> sprites = new Dictionary<Coords, SpriteRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < s; x++)
        {
            for (int y = 0; y < s; y++)
            {
                GameObject inst = Instantiate(prefab, transform);
                inst.transform.position = new Vector3(x, y, 0);
                sprites.Add(new Coords(x, y), inst.GetComponent<SpriteRenderer>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var item in sprites.Values)
            {
                item.color = Color.white;
            }

            for (int x = -a; x < a+1; x++)
            {
                for (int y = -a; y < a+1; y++)
                {
                    if (x > -a && x < a && y > -a && y < a)
                    {
                        continue;
                    }
                    Coords c = new Coords(coords.x+x, coords.y + y);
                    sprites[c].color = Color.red;
                }
            }

            ++a;
        }
    }
}
