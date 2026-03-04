using UnityEngine;

public interface IObstacleMovement
{
    void Move(Transform obj, float speed);
}

public class TranslationMovement : IObstacleMovement
{
    private Vector3 _positionMedian;
    private bool _initialized = false;

    public void Move(Transform obj, float speed)
    {
        if (!_initialized)
        {
            _positionMedian = obj.GetComponent<Obstacle>().StartPosition;
            _initialized = true;
        }
        float x = Mathf.Sin(Time.time * speed) * 10;
        obj.position = _positionMedian + new Vector3(x, 0, 0);
    }
}

public class RotationMovement : IObstacleMovement
{
    public void Move(Transform obj, float speed)
    {
        float rotZ = (Time.deltaTime * speed % 360);
        obj.Rotate(new Vector3(0, 0, rotZ));
    }
}
