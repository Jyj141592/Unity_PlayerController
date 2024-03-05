using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerController{
public class PlayerController : MonoBehaviour
{
    public PlayerControllerAsset playerControllerAsset;

    private void Awake() {
        playerControllerAsset = playerControllerAsset.CloneAsset();
    }
}
}