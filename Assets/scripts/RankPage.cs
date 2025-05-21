using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class RankPage : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] GameObject rowPrefab;

    StageReultList allData;
    int stageindex = 1;
    void Awake()
    {
        allData = StageResultsever.LoadRank();
        RefreshRankList();
    }

    void RefreshRankList()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        var sortedData = allData.reuslt.Where(r => r.stage == stageindex).OrderByDescending(x => x.score).ToList();

        for (int i = 0; i < sortedData.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, contentRoot);
            TMP_Text rankTxet = row.GetComponentInChildren<TMP_Text>();
            rankTxet.text = $"{i + 1}. {sortedData[i].playername} - {sortedData[i].score}";
        }
    }

    
}
