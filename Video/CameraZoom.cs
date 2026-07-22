using Bark.Tool;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Quantum.Video;

[HarmonyPatch(typeof(PlayerCamera))]
public static class CameraZoom
{
    private const float SmoothZoomSpeed = 15f;
    private const float MinZoomMultiplier = 0.25f;
    private const float MaxZoomMultiplier = 8f;

    private static Camera _activeCamera;
    private static Transform _sightLimiter;
    private static float _normalOrthographicSize;
    private static Vector3 _normalSightLimiterScale;
    private static float _currentZoomMultiplier = 1f;
    private static float _targetZoomMultiplier = 1f;
    private static bool _isZoomActive;
    private static float _lastZoomKeyTime;

    private static TextMeshProUGUI _zoomText;
    private static GameObject _zoomUiObject;
    private static bool _isUiVisible;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdatePostfix(PlayerCamera __instance)
    {
        UpdateZoom(__instance);
        UpdateUi();
    }

    private static void UpdateZoom(PlayerCamera playerCamera)
    {
        if (playerCamera == null)
        {
            ReleaseCamera();
            return;
        }

        var renderingCamera = playerCamera.GetComponent<Camera>();
        if (renderingCamera == null)
            renderingCamera = Camera.main;

        if (renderingCamera == null)
        {
            ReleaseCamera();
            return;
        }

        TrackCamera(renderingCamera);
        HandleZoomInput();
        UpdateZoomAnimation();
        ApplyZoom(renderingCamera);
    }

    private static void TrackCamera(Camera renderingCamera)
    {
        if (_activeCamera == renderingCamera)
            return;

        ReleaseCamera();
        _activeCamera = renderingCamera;
        _sightLimiter = renderingCamera.transform.Find("SightLimiter");
        _normalOrthographicSize = renderingCamera.orthographicSize;
        _currentZoomMultiplier = Plugin.ZoomMultiplier;
        _targetZoomMultiplier = Plugin.ZoomMultiplier;

        if (_sightLimiter != null)
            _normalSightLimiterScale = _sightLimiter.localScale;

        RenderPipelineManager.beginCameraRendering += ApplyZoomBeforeRendering;
    }

    private static void HandleZoomInput()
    {
        if (Input.GetKeyDown(Plugin.ZoomKey))
        {
            var currentTime = Time.time;
            if (currentTime - _lastZoomKeyTime < 0.3f)
            {
                _targetZoomMultiplier = 1f;
                Plugin.ZoomMultiplier = 1f;
                _lastZoomKeyTime = 0f;
                return;
            }
            _lastZoomKeyTime = currentTime;
        }

        if (!Input.GetKey(Plugin.ZoomKey))
        {
            _isZoomActive = false;
            return;
        }

        _isZoomActive = true;
        var scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scrollAxis, 0f))
            return;

        _targetZoomMultiplier -= scrollAxis * Plugin.ZoomSensitivity;
        _targetZoomMultiplier = Mathf.Clamp(_targetZoomMultiplier, MinZoomMultiplier, MaxZoomMultiplier);
        Plugin.ZoomMultiplier = _targetZoomMultiplier;
    }

    private static void UpdateZoomAnimation()
    {
        if (!Plugin.SmoothZoom)
        {
            _currentZoomMultiplier = _targetZoomMultiplier;
            return;
        }

        _currentZoomMultiplier = Mathf.Lerp(
            _currentZoomMultiplier,
            _targetZoomMultiplier,
            1f - Mathf.Exp(-SmoothZoomSpeed * Time.unscaledDeltaTime)
        );

        if (Mathf.Abs(_currentZoomMultiplier - _targetZoomMultiplier) < 0.0001f)
            _currentZoomMultiplier = _targetZoomMultiplier;
    }

    private static void ApplyZoomBeforeRendering(ScriptableRenderContext context, Camera camera)
    {
        ApplyZoom(camera);
    }

    private static void ApplyZoom(Camera renderingCamera)
    {
        if (renderingCamera != _activeCamera)
            return;

        renderingCamera.orthographicSize = _normalOrthographicSize * _currentZoomMultiplier;

        if (_sightLimiter == null)
            return;

        _sightLimiter.localScale = new Vector3(
            _normalSightLimiterScale.x * _currentZoomMultiplier,
            _normalSightLimiterScale.y * _currentZoomMultiplier,
            _normalSightLimiterScale.z
        );
    }

    private static void UpdateUi()
    {
        if (!_isZoomActive)
        {
            HideUi();
            return;
        }

        CreateOrUpdateUi();
        ShowUi();
    }

    private static void CreateOrUpdateUi()
    {
        if (_zoomUiObject == null)
        {
            var zoomUi = new GameObject("CameraZoomUi");
            Object.DontDestroyOnLoad(zoomUi);

            var canvas = zoomUi.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var canvasScaler = zoomUi.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);

            zoomUi.AddComponent<GraphicRaycaster>();
            _zoomUiObject = zoomUi;

            var textObj = new GameObject("ZoomText");
            textObj.transform.SetParent(_zoomUiObject.transform, false);

            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(200f, 50f);

            _zoomText = textObj.AddComponent<TextMeshProUGUI>();
            _zoomText.alignment = TextAlignmentOptions.Center;
            _zoomText.font = TextUtil.RetroGamingTMP;
            _zoomText.fontSize = 32;
        }

        _zoomText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, AmmunitionUi.AmunitionUiObject.transform.position.y - 128);
        _zoomText.text = $"{_currentZoomMultiplier:F1}x";
        _zoomText.alpha = HiddenHud.Hidden 
            ? 0f 
            : 1f;
    }

    private static void ShowUi()
    {
        if (_zoomUiObject == null || _isUiVisible)
            return;

        _zoomUiObject.SetActive(true);
        _isUiVisible = true;
    }

    private static void HideUi()
    {
        if (_zoomUiObject == null || !_isUiVisible)
            return;

        _zoomUiObject.SetActive(false);
        _isUiVisible = false;
    }

    private static void ReleaseCamera()
    {
        RenderPipelineManager.beginCameraRendering -= ApplyZoomBeforeRendering;

        if (_sightLimiter != null)
            _sightLimiter.localScale = _normalSightLimiterScale;

        if (_activeCamera != null)
            _activeCamera.orthographicSize = _normalOrthographicSize;

        _activeCamera = null;
        _sightLimiter = null;
        _normalOrthographicSize = 0f;
        _normalSightLimiterScale = Vector3.one;
        _currentZoomMultiplier = Plugin.ZoomMultiplier;
        _targetZoomMultiplier = Plugin.ZoomMultiplier;
        _isZoomActive = false;
    }
}