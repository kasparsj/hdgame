using UnityEngine;
using XXHash = Klak.Math.XXHash;

public class LightBarController : MonoBehaviour
{
    [Space]
    [SerializeField] GameObject _prefab = null;
    [SerializeField] uint _instanceCount = 10;
    public uint randomSeed = 0;
    [Space]
    [SerializeField] float _height = 1;
    [SerializeField] float _width = 50;
    [SerializeField] float _speed = 100;
    [SerializeField] bool _animateY = false;
    [SerializeField] bool _fixRotation = false;

    GameObject[] _bars;

    void Start()
    {
        _bars = new GameObject[_instanceCount];

        var hash = new XXHash(randomSeed);

        for (var i = 0u; i < _instanceCount; i++)
        {
            var go = Instantiate(_prefab, transform);
            var seed = i * 2;
            var hue = hash.Float(seed + 1);
            if (!_animateY) {
                var y = (hash.Float(seed) - 0.5f) * _height;
                go.transform.localPosition = new Vector3(0, y, 0);
            }
            else {
                var x = (hash.Float(seed) - 0.5f) * _width;
                go.transform.localPosition = new Vector3(x, 0, 0);
            }
            go.GetComponent<Light>().color = Color.HSVToRGB(hue, 0.8f, 1);

            _bars[i] = go;
        }
    }

    void Update()
    {
        var hash = new XXHash(randomSeed + 100);
        var t = Time.time;

        for (var i = 0u; i < _instanceCount; i++)
        {
            var p = _bars[i].transform.localPosition;

            var spd = (hash.Float(i) + 0.5f) * _speed;
            if (!_animateY) {
                p.x = ((spd * t) % _width);
            }
            else {
                p.y = ((spd * t) % _height);
            }

            _bars[i].transform.localPosition = p;
            
            if (_fixRotation) {
                Vector3 targetPosition = new Vector3(Camera.main.transform.position.x, 
                                                    _bars[i].transform.position.y, 
                                                    Camera.main.transform.position.z);
                _bars[i].transform.LookAt(targetPosition);
            }
        }
    }
}
