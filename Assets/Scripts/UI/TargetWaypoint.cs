using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetWaypoint : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI _distanceText;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;

    private Camera _camera;

    public void EnableWaypoint()
    {
        enabled = true;
        img.gameObject.SetActive(true);
        _distanceText.gameObject.SetActive(true);
    }

    public void DestroyWaypoint()
    {
        Destroy(img.gameObject);
        Destroy(this);
    }

    public void DisableWaypoint()
    {
        enabled = false;
        img.gameObject.SetActive(false);
        _distanceText.gameObject.SetActive(false);
    }
    
    private void Awake()
    {
        _camera = Camera.main;
    }

    public void SetImage(Image image)
    {
        img = image;
    }

    public void SetText(TextMeshProUGUI text)
    {
        _distanceText = text;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    
    private void Update()
    {
        if (img != null)
        {
            float minX = img.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;

            float minY = img.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector2 pos = _camera.WorldToScreenPoint(target.position + offset);

            if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
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

            img.transform.position = pos;
            float distance = Vector3.Distance(_camera.transform.position, target.position);
            _distanceText.text = $"{Mathf.RoundToInt(distance)}m";
        }
    }
}
