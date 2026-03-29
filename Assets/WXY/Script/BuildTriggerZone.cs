using UnityEngine;

public class BuildTriggerZone : MonoBehaviour
{
    public BuildModeController buildController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            buildController.canEnterBuildMode = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            buildController.canEnterBuildMode = false;
        }
    }
}