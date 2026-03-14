using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
  public BuildingListGenerator BuildingListGenerator;

    private void Awake()
    {
        BuildingListGenerator.OpenMenu("jiajutest");
    }
}
