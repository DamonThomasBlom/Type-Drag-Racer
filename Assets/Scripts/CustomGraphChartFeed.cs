using ChartAndGraph;
using UnityEngine;

public class CustomGraphChartFeed : MonoBehaviour
{
    void Start()
    {
        LoadGraph();
    }

    public void LoadGraph()
    {
        GraphChart graph = GetComponent<GraphChart>();
        if (graph != null)
        {
            graph.DataSource.ClearCategory("Player");
            graph.DataSource.StartBatch(); // start a new update batch
            int counter = 1;

            foreach (int wpm in StatsManager.Instance.playerStats.Last10RacesWPM)
            {
                // If our WPM surpasses the graph max then increase it
                if (wpm > graph.DataSource.VerticalViewSize)
                    graph.DataSource.VerticalViewSize = RoundUpToNearest25(wpm);

                graph.DataSource.AddPointToCategory("Player", counter, wpm);
                counter++;
            }

            graph.DataSource.EndBatch(); // end the update batch . this call will
        }
    }

    float RoundUpToNearest25(float value)
    {
        return Mathf.Ceil(value / 25f) * 25f;
    }
}
