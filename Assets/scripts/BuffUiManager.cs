using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUIManager : MonoBehaviour
{
    [System.Serializable]
    public class BuffInfo
    {
        public string buffName;
        public Sprite icon;
    }

    public GameObject buffSlotPrefab; // 아이콘과 시간 표시가 들어간 프리팹
    public Transform buffContainer;   // UI 부모 오브젝트
    public List<BuffInfo> buffIcons;  // 등록된 버프 이름과 아이콘 리스트

    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>();

    public void ShowBuff(string buffName, float duration)
    {
        // 이미 있으면 제거하고 다시 만들기 (중복 방지)
        if (activeBuffs.ContainsKey(buffName))
        {
            Destroy(activeBuffs[buffName]);
            activeBuffs.Remove(buffName);
        }

        GameObject buffUI = Instantiate(buffSlotPrefab, buffContainer);
        BuffInfo info = buffIcons.Find(b => b.buffName == buffName);

        if (info != null)
        {
            var iconImage = buffUI.transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = info.icon;

            // ▶ 여기 추가! 혹시 투명도 0으로 되어있을 경우 대비
            iconImage.color = Color.white;
        }

        activeBuffs[buffName] = buffUI;
        StartCoroutine(UpdateBuffTimer(buffName, buffUI, duration));

    }

    public void HideBuff(string buffName)
    {
        if (activeBuffs.ContainsKey(buffName))
        {
            Destroy(activeBuffs[buffName]);
            activeBuffs.Remove(buffName);
        }
    }

    private IEnumerator UpdateBuffTimer(string buffName, GameObject uiObject, float duration)
    {
        float remaining = duration;

        TextMeshProUGUI timerText = uiObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        Image icon = uiObject.transform.Find("Icon").GetComponent<Image>();
        Color normalColor = icon.color;

        while (remaining > 0f)
        {
            if (uiObject == null || icon == null || timerText == null)
            {
                yield break;  // 오브젝트가 파괴되었으면 코루틴 중단
            }

            remaining -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(remaining).ToString();

            // 3초 남으면 깜빡이게
            if (remaining <= 3f)
            {
                float alpha = Mathf.PingPong(Time.time * 5f, 1f);
                icon.color = new Color(normalColor.r, normalColor.g, normalColor.b, alpha);
            }

            yield return null;
        }

        HideBuff(buffName); // 끝나면 버프 제거
    }

}
