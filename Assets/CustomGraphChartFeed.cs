using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class CustomGraphChartFeed : MonoBehaviour
{
    void Start()
    {
        GraphChart graph = GetComponent<GraphChart>();
        if (graph != null)
        {
            graph.DataSource.StartBatch(); // start a new update batch
            int counter = 1;
            foreach (int wpm in StatsManager.Instance.playerStats.Last10RacesWPM)
            {
                graph.DataSource.AddPointToCategory("Player", counter, wpm);
                counter++;
            }

            graph.DataSource.EndBatch(); // end the update batch . this call will

        }
    }
}
