using System;
using UnityEngine;

public class StatsRecorder: MonoBehaviour {

    private long[] dt = new long[10 * 100];

    private int i = 0;

    private void Start() {
        dt[i++] = DateTime.Now.Ticks;
    }

    private void Update() {
        if (i != 10 * 100) {
            dt[i++] = DateTime.Now.Ticks;
        }
        else {
            Debug.LogFormat("First frame: {0} ms", (float) (dt[1] - dt[0]) / 10000);
            int m = 0;
            int s = 0;
            for (int i = 1; i != 10 * 100; i++) {
                if (dt[i] - dt[m] > 10000000) {
                    Debug.LogFormat("t: {0}, FPS = {1}", s, i - m);
                    m = i;
                    s++;
                }
            }
            enabled = false;
        }
    }
}
