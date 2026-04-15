using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;
    
    [SerializeField]
    private GameObject _unvisitedBlock;


    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);

    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }


    // loot spawn area code
    public Vector3 GetRandomPosition(float cellSize, float yOffset =0.0f)
    {
        float halfSize = cellSize / 2f;
        float randomX = Random.Range(-halfSize + 0.5f, halfSize - 0.5f);
        float randomZ = Random.Range(-halfSize + 0.5f, halfSize - 0.5f);
        return new Vector3(transform.position.x + randomX, yOffset, transform.position.z + randomZ);
    }

}
