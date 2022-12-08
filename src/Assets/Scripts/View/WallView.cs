using UnityEngine;

public class WallView : MonoBehaviour
{
    public GameObject _rightWall;
    public GameObject _bottomWall;
    public GameObject _leftWall;
    public GameObject _topWall;

    void Start()
    {
        Debug.Assert(_rightWall);
        Debug.Assert(_bottomWall);
        Debug.Assert(_leftWall);
        Debug.Assert(_topWall);

    }

    public void SetRightWallVisible(bool isVisible)
    {
        _rightWall.SetActive(isVisible);
    }

    public void SetBottomWallVisible(bool isVisible)
    {
        _bottomWall.SetActive(isVisible);
    }

    public void SetLeftWallVisible(bool isVisible)
    {
        _leftWall.SetActive(isVisible);
    }

    public void SetTopWallVisible(bool isVisible)
    {
        _topWall.SetActive(isVisible);
    }
}
