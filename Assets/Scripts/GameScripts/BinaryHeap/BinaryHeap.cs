using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于创建A星寻路openList和closeList的小端堆
/// </summary>
public class BinaryHeap
{
    List<AStarGrid> heap;

    public BinaryHeap()
    {
        heap = new List<AStarGrid>();
    }

    public List<AStarGrid> Heap { get => heap; set => heap = value; }
    /// <summary>
    /// 入堆
    /// </summary>
    /// <param name="aStarGrid"></param>
    public void Push(AStarGrid aStarGrid)
    {
        int index = heap.Count;
        heap.Add(aStarGrid);
        while (aStarGrid.F < heap[Mathf.FloorToInt((index - 1) / 2)].F)
        {
            int targetIndex = Mathf.FloorToInt((index - 1) / 2);
            AStarGrid temp = heap[targetIndex];
            heap[targetIndex] = aStarGrid;
            heap[index] = temp;

            index = targetIndex;
        }
    }
    /// <summary>
    /// 出堆
    /// </summary>
    /// <returns></returns>
    public AStarGrid Pop()
    {
        int maxIndex = heap.Count - 1;
        int curIndex = 0;
        int curChildCount;
        AStarGrid result = heap[0];

        //将最后一个元素放在第一位
        heap[0] = heap[maxIndex];
        heap.RemoveAt(maxIndex);

        //将该元素向下放到合适的位置
        //第一步，确定当前节点下还有几个子节点
        curChildCount = CheckChildCount(curIndex);
        //循环，当1.当前节点比左子节点小（1个子节点）2.当前节点比较小的子节点小（2个子节点）3.无子节点（0子节点）时跳出

        while (!(curChildCount == 0 || (curChildCount == 1 && heap[curIndex].F < heap[curIndex * 2 + 1].F) || (curChildCount == 2 && heap[curIndex].F < Mathf.Min(heap[curIndex * 2 + 1].F, heap[curIndex * 2 + 2].F))))
        {
            int targetIndex = 0;
            AStarGrid temp;
            switch (curChildCount)
            {
                case 1:
                    targetIndex = curIndex * 2 + 1;
                    break;
                case 2:
                    targetIndex = heap[curIndex * 2 + 1].F < heap[curIndex * 2 + 2].F ? curIndex * 2 + 1 : curIndex * 2 + 2;
                    break;
            }
            temp = heap[targetIndex];
            heap[targetIndex] = heap[curIndex];
            heap[curIndex] = temp;
            curIndex = targetIndex;
            curChildCount = CheckChildCount(curIndex);

        }

        return result;
    }

    private int CheckChildCount(int curIndex)
    {
        int curChildCount;
        if (curIndex * 2 + 1 > heap.Count - 1)
        {
            curChildCount = 0;
        }
        else
        {
            if (curIndex * 2 + 1 == heap.Count - 1)
            {
                curChildCount = 1;
            }
            else
            {
                curChildCount = 2;
            }
        }

        return curChildCount;
    }
}
