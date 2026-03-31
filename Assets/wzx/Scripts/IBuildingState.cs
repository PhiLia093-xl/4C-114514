using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3 pos , Vector2 box );
    void UpdateState(Vector3 pos , Vector2 box);
}
