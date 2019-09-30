using UnityEngine;

public class TacticsCamera : MonoBehaviour {

	public float RotationAmount = 1.2f;
	void Update() 
	{
		if(Input.GetKey(KeyCode.Q))
		{
			RotateLeft();
		}
		else if(Input.GetKey(KeyCode.E))
		{
			RotateRight();
		}
	}
	public void RotateLeft()
	{
		transform.Rotate(Vector3.up, RotationAmount, Space.Self);
	}

	public void RotateRight()
	{
		transform.Rotate(Vector3.up, -RotationAmount, Space.Self);
	}
}
