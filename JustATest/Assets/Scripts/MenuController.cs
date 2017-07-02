﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public uint amountOfOptions = 4;
    public uint offsetOnSelection = 150;
    public RectTransform selectionArrow;
    public RectTransform[] buttonTransforms;
    public GameObject menuPanel, controlsPanel, creditsPanel;
    public AudioSource arrowMoveSound, selectionSound, beginGameSound;
    public float selectionCooldown = 0.1f;

    private RectTransform rect;
    private uint selectedOption = 0;
    private float selectionTimer = 0.0f;

    void Update() {
        HandleButtons();

        if (selectionTimer > 0.0f) {
            selectionTimer -= Time.deltaTime;
            return;
        }

        if (Input.GetAxis("Vertical Menu") < -0.1 && selectedOption > 0) {
            selectedOption--;
            selectionArrow.position = new Vector3(selectionArrow.position.x, buttonTransforms[selectedOption].position.y, selectionArrow.position.z);
            selectionTimer = selectionCooldown;
            arrowMoveSound.Play();
        }
        else if (Input.GetAxis("Vertical Menu") > 0.1f && selectedOption < amountOfOptions - 1) {
            selectedOption++;
            selectionArrow.position = new Vector3(selectionArrow.position.x, buttonTransforms[selectedOption].position.y, selectionArrow.position.z);
            selectionTimer = selectionCooldown;
            arrowMoveSound.Play();
        }
    }

    void HandleButtons() {
        if (Input.GetButtonDown("Back")) {
            menuPanel.SetActive(true);
            controlsPanel.SetActive(false);
            creditsPanel.SetActive(false);
            selectionSound.Play();
        }
        else if (Input.GetButtonDown("Select")) {
            switch (selectedOption) {
                case 0: beginGameSound.Play(); SceneManager.LoadScene(1); break;
                case 1: selectionSound.Play(); menuPanel.SetActive(false); controlsPanel.SetActive(true); creditsPanel.SetActive(false); break;
                case 2: selectionSound.Play(); menuPanel.SetActive(false); controlsPanel.SetActive(false); creditsPanel.SetActive(true); break;
                case 3: selectionSound.Play(); Application.Quit(); break;
                default: break;
            }
        }
    }
}
