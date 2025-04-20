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

    public GameObject buffSlotPrefab; // �����ܰ� �ð� ǥ�ð� �� ������
    public Transform buffContainer;   // UI �θ� ������Ʈ
    public List<BuffInfo> buffIcons;  // ��ϵ� ���� �̸��� ������ ����Ʈ

    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>();

    public void ShowBuff(string buffName, float duration)
    {
        // �̹� ������ �����ϰ� �ٽ� ����� (�ߺ� ����)
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

            // �� ���� �߰�! Ȥ�� ���� 0���� �Ǿ����� ��� ���
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
                yield break;  // ������Ʈ�� �ı��Ǿ����� �ڷ�ƾ �ߴ�
            }

            remaining -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(remaining).ToString();

            // 3�� ������ �����̰�
            if (remaining <= 3f)
            {
                float alpha = Mathf.PingPong(Time.time * 5f, 1f);
                icon.color = new Color(normalColor.r, normalColor.g, normalColor.b, alpha);
            }

            yield return null;
        }

        HideBuff(buffName); // ������ ���� ����
    }

}
