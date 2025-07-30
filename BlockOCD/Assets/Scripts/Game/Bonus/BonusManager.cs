using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    private void Start()
    {
        GameEvent.ShowBonusWriting += ShowBonusWriting;
    }

    private void OnDisable()
    {
        GameEvent.ShowBonusWriting -= ShowBonusWriting;
    }

    private void ShowBonusWriting(Config.SquareColor color)
    {
        GameObject obj = null;

        foreach (GameObject bonus in bonusList)
        {
            Bonus bonusComponent = bonus.GetComponent<Bonus>();
            if (bonusComponent.color == color)
            {
                obj = bonus;
                bonus.SetActive(true);
            }
        }
        StartCoroutine(DiactivateBonusWriting(obj));
    }

    private IEnumerator DiactivateBonusWriting(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        if(obj != null)
            obj.SetActive(false);
    }
}
