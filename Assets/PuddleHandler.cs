using UnityEngine;
using DG.Tweening;
public class PuddleHandler : MonoBehaviour
{
    [SerializeField] LayerMask TargetLayer;
    void Start()
    {
        ShufflePuddle();
    }
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & TargetLayer) != 0)
        {
            Stumble();
        }
    }
    public void ShufflePuddle()
    {
        int RandomMaterial = Random.Range(0, GameManager.Instance.PuddleMaterials.Length);
        GetComponent<MeshRenderer>().material = GameManager.Instance.PuddleMaterials[RandomMaterial];  
    }
    void Stumble()
    {
        PlayerController.instance.canMovement = false;
        PlayerController.instance.animator.SetTrigger("Stumble");
        PlayerController.instance.moveDirection.x = 0f;
        PlayerController.instance.moveForwardSpeed = 7f;
    }

}
