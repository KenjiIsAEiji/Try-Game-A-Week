using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [Header("UI elements")]
    [SerializeField] Text BulletsText;
    [SerializeField] Gradient BulletsTextColor;
    [SerializeField] Text reloadWarning;
    [SerializeField] Text reloadingText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        string nowBulletsTxt = "<b>" + player.magazine + "</b>";
        
        nowBulletsTxt += "<size=BulletsText.fontSize / 2>/" + player.MaxBullets + "</size>";

        BulletsText.text = nowBulletsTxt;
        float amount = 1f - ((float)player.magazine / (float)player.MaxBullets);
        BulletsText.color = BulletsTextColor.Evaluate(amount);

        reloadWarning.gameObject.SetActive(amount >= 1 && !player.reloading);

        reloadingText.gameObject.SetActive(player.reloading);
    }
}
