using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerStatus status;
    [Header("StatusUI")]
    [SerializeField] Image LpBar;
    [SerializeField] Text LpParcent;
    [SerializeField] Color LpWarningColor;
    [Header("BulletsHUD")]
    [SerializeField] Image BulletHUDBoder;
    [SerializeField] Text BulletsText;
    [SerializeField] Gradient BulletsTextColor;
    [SerializeField] Text reloadWarning;
    [SerializeField] Text reloadingText;
    [Header("RaizerHUD")]
    [SerializeField] Image RaizerHUDBoder;
    [SerializeField] Text rechargeWarning;
    [SerializeField] Text readyText;
    [SerializeField] Image progressBar;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        RaizerHUDBoder.enabled = player.RaizerMode;
        BulletHUDBoder.enabled = !player.RaizerMode;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float lpAmount = status.PlayerLp / status.PlayerMaxLp;
        LpBar.fillAmount = Mathf.Lerp(0f, 0.5f,lpAmount);
        LpParcent.text = (lpAmount * 100).ToString("00.0") + "%";

        if (status.LpWarning)
        {
            LpParcent.color = LpWarningColor;
        }
        else
        {
            LpParcent.color = Color.white;
        }

        //
        string nowBulletsTxt = "<b>" + player.magazine + "</b>";
        
        nowBulletsTxt += "<size=BulletsText.fontSize / 2>/" + player.MaxBullets + "</size>";

        BulletsText.text = nowBulletsTxt;
        float amount = 1f - ((float)player.magazine / (float)player.MaxBullets);
        BulletsText.color = BulletsTextColor.Evaluate(amount);

        reloadWarning.gameObject.SetActive(amount >= 1 && !player.reloading);
        reloadingText.gameObject.SetActive(player.reloading);

        //
        float fill = player.Casting / player.RecastTime;
        progressBar.fillAmount = fill;

        readyText.gameObject.SetActive(fill >= 1f);
        rechargeWarning.gameObject.SetActive(!(fill >= 1f));
    }
}
