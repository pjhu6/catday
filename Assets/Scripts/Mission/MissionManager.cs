using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using StarterAssets;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup missionCanvas;
    [SerializeField] private CanvasGroup missionTextCanvas;
    [SerializeField] private TMP_Text missionDescription;
    [SerializeField] private TMP_Text missionTitle;
    [SerializeField] private Image checkboxImage;
    [SerializeField] private CanvasGroup checkboxCanvasGroup;
    [SerializeField] private float completedMissionDelay = 1f;

    [SerializeField] private List<MissionData> remainingMissions;

    private List<MissionData> completedMissions;
    private FirstPersonController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        completedMissions = new List<MissionData>();
        checkboxImage.enabled = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UpdateMissionText();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            // Refresh UI references if they're missing
            // if (missionTitle == null)
            //     missionTitle = GameObject.Find("MissionTitleText")?.GetComponent<TMP_Text>();
            if (missionDescription == null)
                missionDescription = GameObject.Find("MissionDescriptionText")?.GetComponent<TMP_Text>();
            if (missionCanvas == null)
                missionCanvas = GameObject.Find("MissionsPanel")?.GetComponent<CanvasGroup>();
            if (missionTextCanvas == null)
                missionTextCanvas = GameObject.Find("MissionsTextPanel")?.GetComponent<CanvasGroup>();
            if (checkboxImage == null)
                checkboxImage = GameObject.Find("MissionCheckboxImage")?.GetComponent<Image>();
            if (checkboxCanvasGroup == null)
                checkboxCanvasGroup = checkboxImage?.GetComponent<CanvasGroup>();
            UpdateMissionText();
        }
    }

    public void CompleteMission(string missionId)
    {
        MissionData mission = remainingMissions.Find(m => m.id == missionId);
        if (mission != null)
        {
            remainingMissions.Remove(mission);
            AddToCompletedMissions(mission);
            UpdateMissionText();
        }
    }

    private void AddToCompletedMissions(MissionData mission)
    {
        completedMissions.Add(mission);
    }

    private void UpdateMissionText()
    {
        checkboxImage.enabled = false;
        checkboxCanvasGroup.alpha = 0f;
        StartCoroutine(AnimateMissionUpdate());
    }

    private IEnumerator AnimateMissionUpdate()
    {
        if (completedMissions.Count > 0)
        {
            MissionData lastCompleted = completedMissions[completedMissions.Count - 1];
            // missionTitle.text = lastCompleted.title;
            missionDescription.text = lastCompleted.description;

            if (lastCompleted.respawnPlayer && lastCompleted.respawnPosition != Vector3.zero)
            {
                yield return new WaitForSeconds(0.1f);
                TeleportPlayer(lastCompleted.respawnPosition);
            }

            // Show and fade in checkbox
            if (checkboxImage != null && checkboxCanvasGroup != null)
            {
                checkboxImage.enabled = true;
                checkboxCanvasGroup.alpha = 0f;

                for (float t = 0f; t <= 1f; t += Time.deltaTime * 1.5f)
                {
                    checkboxCanvasGroup.alpha = t;
                    yield return null;
                }
                checkboxCanvasGroup.alpha = missionTextCanvas.alpha;
            }

            yield return new WaitForSeconds(completedMissionDelay);

            // Fade out mission panel
            float initialAlpha = missionTextCanvas.alpha;
            for (float t = initialAlpha; t >= 0f; t -= Time.deltaTime / 2)
            {
                missionTextCanvas.alpha = t;
                checkboxCanvasGroup.alpha = t;
                yield return null;
            }
        }

        checkboxImage.enabled = false;

        // Update to next mission
        if (remainingMissions.Count > 0)
        {
            // missionTitle.text = remainingMissions[0].title;
            missionDescription.text = remainingMissions[0].description;
        }
        else
        {
            missionTitle.text = "Congratulations!";
            missionDescription.text = "All missions completed!";
        }

        // Fade in mission panel
        for (float t = 0f; t <= 1f; t += Time.deltaTime)
        {
            missionTextCanvas.alpha = t;
            yield return null;
        }

        missionTextCanvas.alpha = 1f;
    }

    public void DisplayMissionPanel()
    {
        // enable the mission panel with the fade in effect
        if (missionCanvas != null)
        {
            StartCoroutine(FadeInMissionPanel());
        }
    }

    private IEnumerator FadeInMissionPanel()
    {
        missionCanvas.alpha = 0f;
        missionCanvas.gameObject.SetActive(true);

        for (float t = 0f; t <= 1f; t += Time.deltaTime)
        {
            missionCanvas.alpha = t;
            yield return null;
        }

        missionCanvas.alpha = 1f;
    }

    public void HideMissionPanel()
    {
        if (missionCanvas != null)
        {
            missionCanvas.gameObject.SetActive(false);
        }
    }

    public void TeleportPlayer(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                Vector3 worldPosition = player.transform.parent.TransformPoint(position);
                playerController.TeleportTo(worldPosition);
            }
        }
    }

    public bool IsMissionCompleted(string missionId)
    {
        return completedMissions.Exists(m => m.id == missionId);
    }
}
