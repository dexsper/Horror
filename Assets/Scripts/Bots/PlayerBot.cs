using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBot : Bot
{
    [SerializeField] private List<string> _names;
    [SerializeField] private TextMeshProUGUI _nameText;
    
    //private Door _door;

    protected override void Awake()
    {
        base.Awake();

        if(_nameText != null)
        {
            _nameText.text = _names[Random.Range(0, _names.Count)];
        }
    }

    /*protected override Vector3 GetTargetPosition()
    {
        if (!_playerBehaviour.KeyTaken)
        {
            var key = _fieldOfView.GetObject<Key>();

            if (key != null)
            {
                return key.transform.position;
            }
        }

        if (_door == null)
        {
            var door = _fieldOfView.GetObject<Door>();

            if (door != null)
            {
                _door = door;
            }
        }

        if (_door != null && (_playerBehaviour.KeyTaken || _door.IsOpen))
        {
            return _door.transform.position;
        }

        return Vector3.zero;
    }*/
}
