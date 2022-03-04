using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[View]
[System.Serializable]
public class PlayerView : MonoBehaviour
{
    //tmp
    int i;

    public PlayerController player_con;
    public Text lifeText;   //������ ǥ��
    public Text coinText;   //���� ���� ���� ǥ��
    public Slider hpIcon_sd; //ü�¹� ǥ��

    public void onInit(float _life, float _hp, float _hpmax)
    {
        OnLifeChanged(_life);
        OnHpChanged(_hp, _hpmax, false);
    }

    // ȭ�鿡 ������ ǥ��
    public void OnLifeChanged(float _life)
    {
        lifeText.text = $"Life x {_life}";
    }

    //ü�� ǥ�� ����
    public void OnHpChanged(float _hp, float _hpmax, bool isSlide)
    {
        hpIcon_sd.maxValue = _hpmax;
        StopCoroutine("hpChange");
        if (isSlide)
            StartCoroutine(hpChange(_hp));
        else
            hpIcon_sd.value = _hp;
    }
    IEnumerator hpChange(float _hp)
    {
        float currhp = hpIcon_sd.value;
        float addhpcount = (_hp - currhp) * 0.01f;
        bool isHigh = addhpcount < 0 ? true : false;
        while (true)
        {
            if (isHigh)
            {
                if (currhp <= _hp)
                    break;
            }
            else
            {
                if (currhp >= _hp)
                    break;
            }
            currhp += addhpcount;
            hpIcon_sd.value = currhp;
            yield return null;
        }
        hpIcon_sd.value = _hp;
    }

    public void OnCoinChanged(int _coin)
    {
        coinText.text = $"Coin x {_coin}";
    }

    public void IsHurtEnd()
    {
        player_con.IsHurtEnd();
    }
    public void IsDeadEnd()
    {
        player_con.IsDeadEnd();
    }
}