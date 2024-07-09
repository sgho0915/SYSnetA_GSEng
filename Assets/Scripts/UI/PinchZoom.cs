using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    // 줌 대상 RectTransform. 에디터에서 연결해줄 것
    [SerializeField] private RectTransform _zoomTargetRt;

    // 최대 확대 배율
    private readonly float _ZOOM_IN_MAX = 10f;
    // 최대 축소 배율
    private readonly float _ZOOM_OUT_MAX = 1f;
    // 줌 스피드 조절 값
    private readonly float _ZOOM_SPEED = 1f;

    // 현재 줌 동작 중인지 여부
    private bool _isZooming = false;

    private void Update()
    {
        // 두 개의 터치 입력이 감지되면
        if (Input.touchCount == 2)
        {
            ZoomAndPan();  // 줌 및 팬 동작 처리
        }
        else
        {
            _isZooming = false;
        }
    }

    private void ZoomAndPan()
    {
        if (_isZooming == false)
        {
            _isZooming = true;
        }

        // 현재 터치 위치와 이전 터치 위치 계산
        var prevTouchAPos = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
        var prevTouchBPos = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;
        var curTouchAPos = Input.GetTouch(0).position;
        var curTouchBPos = Input.GetTouch(1).position;

        // 두 터치 간의 거리 변화를 계산하여 줌 양을 결정
        var deltaDistance =
            Vector2.Distance(Normalize(curTouchAPos), Normalize(curTouchBPos))
            - Vector2.Distance(Normalize(prevTouchAPos), Normalize(prevTouchBPos));

        // 현재 스케일 값
        var currentScale = _zoomTargetRt.localScale.x;
        // 줌 양 결정
        var zoomAmount = deltaDistance * currentScale * _ZOOM_SPEED;

        // 확대/축소 값의 범위 제한
        var zoomedScale = currentScale + zoomAmount;
        if (zoomedScale < _ZOOM_OUT_MAX)
        {
            zoomedScale = _ZOOM_OUT_MAX;
            zoomAmount = 0f;
        }
        if (_ZOOM_IN_MAX < zoomedScale)
        {
            zoomedScale = _ZOOM_IN_MAX;
            zoomAmount = 0f;
        }
        _zoomTargetRt.localScale = zoomedScale * Vector3.one;

        // 스케일 변경으로 인한 위치 보정
        var pivotPos = _zoomTargetRt.anchoredPosition;
        var fromCenterToInputPos = new Vector2(
                Input.mousePosition.x - Screen.width * 0.5f,
                Input.mousePosition.y - Screen.height * 0.5f);
        var fromPivotToInputPos = fromCenterToInputPos - pivotPos;
        var offsetX = (fromPivotToInputPos.x / zoomedScale) * zoomAmount;
        var offsetY = (fromPivotToInputPos.y / zoomedScale) * zoomAmount;
        _zoomTargetRt.anchoredPosition -= new Vector2(offsetX, offsetY);

        // 터치의 이동 거리 계산
        var deltaPosTouchA = Input.GetTouch(0).deltaPosition;
        var deltaPosTouchB = Input.GetTouch(1).deltaPosition;
        var deltaPosTotal = (deltaPosTouchA + deltaPosTouchB) * 0.5f;
        var moveAmount = new Vector2(deltaPosTotal.x, deltaPosTotal.y);

        // 패닝 범위 제한
        var clampX = (Screen.width * zoomedScale - Screen.width) * 0.5f;
        var clampY = (Screen.height * zoomedScale - Screen.height) * 0.5f;
        var clampedPosX = Mathf.Clamp(_zoomTargetRt.localPosition.x + moveAmount.x, -clampX, clampX);
        var clampedPosY = Mathf.Clamp(_zoomTargetRt.localPosition.y + moveAmount.y, -clampY, clampY);
        _zoomTargetRt.anchoredPosition = new Vector3(clampedPosX, clampedPosY);
    }

    // 화면 중심을 기준으로 입력 위치를 정규화
    private Vector2 Normalize(Vector2 position)
    {
        var normlizedPos = new Vector2(
            (position.x - Screen.width * 0.5f) / (Screen.width * 0.5f),
            (position.y - Screen.height * 0.5f) / (Screen.height * 0.5f));
        return normlizedPos;
    }
}