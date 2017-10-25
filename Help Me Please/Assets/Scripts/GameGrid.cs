﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{

    public int xSize, ySize;
    public float candyWidth = 1f;
    private GameObject[] _candies;
    private GridItem[,] _items;
    private GridItem _currentlySelectedItem;

    private void Start()
    {
        GetCandies();
        FillGrid();
        GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
        List<GridItem> matchesForItem = SearchVertically(_items[3, 3]);
        if(matchesForItem.Count >=3)
        {
            Debug.Log("There are valid matches in the index."); // 색인에 유효한 일치가 있다
        }
        else
        {
            Debug.Log("There are no valid matches in the index."); // 색인에 유효한 일치 항목이 없습니다.
        }
    }

    void OnDisable()
    {
        GridItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }

    void FillGrid()
    {
        _items = new GridItem[xSize, ySize];
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                //생성
                _items[x, y] = InstantiateCandy(x , y);
            }
        }
    }

    GridItem InstantiateCandy(int x, int y)
    {
        GameObject randomCandy = _candies[Random.Range(0, _candies.Length)];
        GridItem newCandy = ((GameObject)Instantiate(randomCandy, new Vector3(x * candyWidth, y), Quaternion.identity)).GetComponent<GridItem>();
        newCandy.OnItemPositionChanged(x, y);
        return newCandy;
    }

    void OnMouseOverItem(GridItem item)
    {
        if (_currentlySelectedItem == null)
        {
            _currentlySelectedItem = item;
        }
        else
        {
            float xDiff = Mathf.Abs(item.x - _currentlySelectedItem.x);
            float yDiff = Mathf.Abs(item.y - _currentlySelectedItem.y);
            if (xDiff + yDiff == 1)
            {
                StartCoroutine(Swap(_currentlySelectedItem, item));
                _currentlySelectedItem = null;
            }
            else
            {
                Debug.LogError("Esses items");
            }
            _currentlySelectedItem = null;
        }
    }

    IEnumerator Swap(GridItem a, GridItem b)
    {
        ChangeRigidbodyStatus(false);
        float movDuration = 0.1f;
        Vector3 aPosition = a.transform.position;
        StartCoroutine(a.transform.Move(b.transform.position, movDuration));
        StartCoroutine(b.transform.Move(aPosition, movDuration)); //바꾸었을 때 위치 조정
        yield return new WaitForSeconds(movDuration);
        SwapIndices(a, b);
        ChangeRigidbodyStatus(true);

    }

    void SwapIndices(GridItem a, GridItem b)
    {
        GridItem tempA = _items[a.x, a.y];
        _items[a.x, a.y] = b;
        _items[b.x, b.y] = tempA;
        int bOldX = b.x; int bOldY = b.y;
        b.OnItemPositionChanged(a.x, a.y);
        a.OnItemPositionChanged(bOldX, bOldY);
    }

    List<GridItem> SearchHorizontally(GridItem item)
    {
        List<GridItem> hItems = new List<GridItem> { item };
        int left = item.x - 1;
        int right = item.x + 1;
        while(left>=0 && _items[left,item.y].id == item.id)
        { 
            hItems.Add(_items[left, item.y]);
            left--;
        }
        while(right<xSize&& _items [right, item.y].id ==item.id)
        {
            hItems.Add(_items[right, item.y]);
            right++;
        }
        return hItems;
    }

    List<GridItem> SearchVertically (GridItem item)
    {
        List<GridItem> vItems = new List<GridItem> { item };
        int lower = item.y - 1;
        int upper = item.y + 1;
        while(lower>=0 && _items[item.x, lower].id==item.id)
        {
            vItems.Add(_items[item.x, lower]);
            lower--;
        }
        while(upper<ySize && _items[item.x, upper].id==item.id)
        {
            vItems.Add(_items[item.x, upper]);
            upper++;
        }
        return vItems;
    }

    void GetCandies()
    {
        _candies = Resources.LoadAll<GameObject>("Prefabs");
        for (int i = 0; i < _candies.Length; i++)
        {
            _candies[i].GetComponent<GridItem>().id = i;
        }
    }

    void ChangeRigidbodyStatus(bool status)
    {
        foreach (GridItem g in _items)
        {
            g.GetComponent<Rigidbody2D>().isKinematic = !status;
        }
    }
}
