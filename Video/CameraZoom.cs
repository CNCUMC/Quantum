using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

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

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdatePostfix(PlayerCamera __instance)
    {
        UpdateZoom(__instance);
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
        // 按中键重置缩放
        if (Input.GetMouseButtonDown(2))
        {
            _targetZoomMultiplier = 1f;
            Plugin.ZoomMultiplier = 1f;
            return;
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

        // 向上滚动（正值）= 放大（缩小 orthographicSize）
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