using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class VitalGauge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public Vital vital = new Vital ();

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (!eventData.dragging) {

			Vital _vital = this.vital;

			VitalsUI.ins.UpdateVitalDetails (_vital);
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		
	}
}
