using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationButton : MonoBehaviour {
	static bool prev = false;
	static bool now = false;

	void Start() {
		DontDestroyOnLoad(this.gameObject);
	}

	void Update() {
		prev = now;
		now = (Input.GetAxis("Fire2") > 0);
	}
	public static bool GetCalibrationButtonDown() {
		return !prev && now;
	}
	public static bool GetCalibrationButton() {
		return now;
	}
	public static bool GetCalibrationButtonUp() {
		return prev && !now;
	}
}
