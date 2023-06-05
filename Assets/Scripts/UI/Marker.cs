using System.Collections;
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
        _image.gameObject.SetActive(false);
        _distanceText.gameObject.SetActive(false);
        StartCoroutine(EnableWaypoint());
    } 

    private void FixedUpdate()
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
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            _image.transform.position = pos;
            float distance = Vector3.Distance(_camera.transform.position, _target.transform.position);
            _distanceText.text = $"{Mathf.RoundToInt(distance)}m";
        }
    }

    private IEnumerator EnableWaypoint()
    {
        yield return new WaitForSeconds(0.2f);
        _image.gameObject.SetActive(true);
        _distanceText.gameObject.SetActive(true);
    }
}