using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VRUIItem : MonoBehaviour
{
	private BoxCollider boxCollider;
	private RectTransform rectTransform;

	private void OnEnable()
	{
		ValidateCollider();
	}

	private void OnValidate()
	{
		ValidateCollider();
	}

	private void ValidateCollider()
	{
        Debug.Log("Collide???");
		rectTransform = GetComponent<RectTransform>();

		boxCollider = GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
            Debug.Log("Collide");
			boxCollider = gameObject.AddComponent<BoxCollider>();
		}

		boxCollider.size = rectTransform.sizeDelta;
	}
}
