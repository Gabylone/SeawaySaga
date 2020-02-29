[System.Serializable]
public class Node {

	public string name;
	public int row, col;

	public Node () {
		
	}

	public Node ( string n, int p1 , int p2 ) {
		name = n;
		row = p1;
		col = p2;
	}

}