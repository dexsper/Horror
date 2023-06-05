using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private Vector3 _offset;

    private Camera _camera; 
    private MarkerTarget _target;
    
    public void DisableMarker()
    {
        _target = null;
        gameObject.SetActive(false);
    }
    
    private void Awake()
    {
        _camera = Camera.main;
    }

    public void SetTarget(MarkerTarget target)
    {
        _target = target;
        _target.SetMarker(this);
    } 

    private void Update()
    {
        if (_target != null && _image != null) 
        {
            float minX = _image.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = _image.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector2 pos = _camera.WorldToScreenPoint(_target.transform.position + _offset);

            if (Vector3.Dot((_target.transform.position - transform.position), transform.forward) < 0)
            {
                if (pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
                if (pos.y < Screen.height / 2)
                {
                    pos.y = maxY;
                }
                else
                {
                    pos.y = minY;
                }
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            _image.transform.position = pos;
            float distance = Vector3.Distance(_camera.transform.position, _target.transform.position);
            _distanceText.text = $"{Mathf.RoundToInt(distance)}m";
        }
    }
}