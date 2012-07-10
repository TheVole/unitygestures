using UnityEngine;
using System.Collections;

public class TouchPlay : MonoBehaviour
{
	
	public UILabel label;
	public int puckLayer = 8;
	public GameObject marker;
	public GameObject markerDisc;
	Transform lastPuck = null;
	public Material baseMaterial;
	float baseHeight = 0.0f;
	
	private bool isDragging = false;
	
	void DisplayText (string text)
	{
		if (label) {
			label.text = text;
		} else {
			Debug.Log (text);
		}
	}

	void OnEnable ()
	{
		Gesture.onDraggingStartE += HandleGestureonDraggingStartE;
		Gesture.onMultiTapE += HandleGestureonMultiTapE;
		Gesture.onDraggingE += HandleDragging;
		Gesture.onMFDraggingE += HandleDragging;
		Gesture.onDraggingEndE += HandleGestureonDraggingEndE;
		Gesture.onSwipeE += HandleSwipe;
		Gesture.onTouchE += HandleGestureonTouchE;
		Gesture.onTouchDownE += HandleGestureonTouchDownE;
	}


	
	void OnDisable ()
	{
		Gesture.onMultiTapE -= HandleGestureonMultiTapE;
		Gesture.onDraggingE -= HandleDragging;
		Gesture.onMFDraggingE -= HandleDragging;
		Gesture.onDraggingStartE -= HandleGestureonDraggingStartE;
		Gesture.onDraggingEndE -= HandleGestureonDraggingEndE;
		Gesture.onSwipeE -= HandleSwipe;
		Gesture.onTouchE -= HandleGestureonTouchE;
		Gesture.onTouchDownE -= HandleGestureonTouchDownE;
	}
	
	void ShowMarkerAt(Vector2 position)
	{
		Camera mainCamera = Camera.main;
		float pos = mainCamera.transform.position.y - baseHeight - 1.0f;
		Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3 (position.x, position.y, pos));
		marker.transform.position = worldPos;
	}
	
	void HandleSwipe (SwipeInfo sw)
	{
		if (isDragging) return;
		Transform puck = PickObject (sw.startPoint);		
		if (puck) {					
			markerDisc.transform.position = puck.position;
			ResetPuck();
			DisplayText ("From: " + sw.startPoint + " to " + sw.endPoint + " Speed: " + sw.speed + " Angle: " + sw.angle);		
			PuckMover mover = puck.GetComponent<PuckMover>();
			mover.Move(sw.speed, sw.angle);
		}		
	}
	
	void HandleGestureonDraggingStartE (DragInfo dragInfo)
	{
		if (!isDragging) return;
		Transform puck = PickObject (dragInfo.pos);
		if (puck) {
			lastPuck = puck;		
			baseHeight = puck.position.y;		
			puck.renderer.material.color = Color.green;
		}
	}
	
	void ResetPuck() 
	{
		if (lastPuck) {
			if (baseMaterial)
				lastPuck.renderer.material.color = baseMaterial.color;
			lastPuck.rigidbody.velocity = Vector3.zero;
			lastPuck = null;
		}
	}
	
	void HandleGestureonDraggingEndE (DragInfo dragInfo)
	{
		ResetPuck();
		
	}

	void HandleDragging (DragInfo dragInfo)
	{
		if (!isDragging) return;
		if (lastPuck) {	
			DisplayText ("Dragging puck to: " + dragInfo.pos);
			Vector3 worldPos = GetWorldPos (dragInfo.pos);
			lastPuck.transform.position = worldPos;
		} else {
			DisplayText ("Dragging to: " + dragInfo.pos);
		}
	}

	void HandleGestureonMultiTapE (Tap tap)
	{
		if (!PickObject(tap.pos)) {
			if (tap.count == 2) {
				isDragging = !isDragging;
				DisplayText("Dragging " + (isDragging ? "on" : "off"));
			}
		}
			
	}
	
	
	void HandleGestureonTouchE (Vector2 pos)
	{
		Transform puck = PickObject(pos);
		if (puck) {
			puck.rigidbody.velocity = Vector3.zero;
		}
	}
	
	
	void HandleGestureonTouchDownE (Vector2 pos)
	{
		ShowMarkerAt(pos);
	}
	
	Vector3 GetWorldPos (Vector2 screenPos)
	{
		Camera mainCamera = Camera.main;
		float pos = mainCamera.transform.position.y - baseHeight - lastPuck.localScale.y;
		return mainCamera.ScreenToWorldPoint (new Vector3 (screenPos.x, screenPos.y, pos));
	}
	
	Transform PickObject (Vector2 pos)
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay (pos);
		RaycastHit hit;
		int layerMask = 1 << puckLayer;
		Transform result = null;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
			result = hit.transform;
		}
		return result;
	}

}