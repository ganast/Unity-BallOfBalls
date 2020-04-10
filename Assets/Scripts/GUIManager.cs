using System;
using System.Timers;
using UnityEngine;
using UnityEditor;
using Unity.Collections;

public class GUIManager : MonoBehaviour {

    private static readonly int FPS_BUFFER_LENGTH = 30;

    private static readonly int DASHBOARD_WIDTH = 200;
    private static readonly int DASHBOARD_HEIGHT = 393;

    private static readonly int FPS_CHART_COLOR_THRESHOLD = 156;

    private string sNS;
    private string sU0;
    private string sFF;
    private string sSE;
    private string sBS;

    private Rect rectDashboard = new Rect(10, 10, DASHBOARD_WIDTH, DASHBOARD_HEIGHT);

    private Texture2D tex;

    private GUIContent gfps;

    private GUIStyle guiStyleFPS;
    private GUIStyle guiStyleTF;
    private GUIStyle guiStyleLBL;
    private GUIStyle guiStyleBTN;

    private int fpsi;
    private int[] fpsv = new int[FPS_BUFFER_LENGTH];

    private int frames;

    private long tfps;
    private long ttex;

    public void Start() {

        SimManager sm = SimManager.GetSimManager();

        sNS = sm.GetSpawnCount().ToString();
        sU0 = sm.GetInitialSpeed().ToString();
        sFF = sm.GetForceFactor().ToString();
        sSE = sm.GetSpawnExtent().ToString();
        sBS = sm.GetBallScale().ToString();

        tex = new Texture2D(fpsv.Length, 200, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;

        gfps = new GUIContent();

        guiStyleFPS = null;
        guiStyleTF = null;
        guiStyleLBL = null;
        guiStyleBTN = null;

        fpsi = 0;
        fpsv[fpsi] = 0;

        frames = 0;

        tfps = 0;
        ttex = 0;

        Timer timer = new Timer(500);
        timer.Elapsed += UpdateFPS;
        timer.Enabled = true;
    }

    public void Update() {

        frames++;

        if (tfps >= ttex) {
            RebuildFPSTex();
            ttex = DateTime.Now.Ticks;
        }
    }

    public void OnGUI() {

        if (guiStyleFPS == null) {
            guiStyleFPS = new GUIStyle(GUI.skin.textField);
            guiStyleFPS.fontSize = 32;
            guiStyleFPS.fontStyle = FontStyle.Bold;
            guiStyleFPS.normal.textColor = Color.green;
            guiStyleFPS.margin = new RectOffset(0, 0, 0, 0);
            guiStyleFPS.padding = new RectOffset(10, 10, 5, 5);
        }

        if (guiStyleTF == null) {
            guiStyleTF = new GUIStyle(GUI.skin.textField);
            guiStyleTF.fontSize = 12;
            guiStyleTF.fontStyle = FontStyle.Bold;
            guiStyleTF.normal.textColor = Color.green;
            guiStyleTF.margin = new RectOffset(0, 0, 5, 0);
            guiStyleTF.padding = new RectOffset(5, 5, 5, 5);
        }

        if (guiStyleLBL == null) {
            guiStyleLBL = new GUIStyle(GUI.skin.label);
            guiStyleLBL.fontSize = 12;
            guiStyleLBL.fontStyle = FontStyle.Bold;
            guiStyleLBL.normal.textColor = Color.green;
            guiStyleLBL.margin = new RectOffset(0, 0, 5, 0);
            guiStyleLBL.padding = new RectOffset(5, 5, 5, 5);
        }

        if (guiStyleBTN == null) {
            guiStyleBTN = new GUIStyle(GUI.skin.button);
            guiStyleBTN.fontSize = 12;
            guiStyleBTN.fontStyle = FontStyle.Bold;
            guiStyleBTN.normal.textColor = Color.green;
            guiStyleBTN.margin = new RectOffset(0, 0, 5, 0);
            guiStyleBTN.padding = new RectOffset(5, 5, 5, 5);
        }

        SimManager sm = SimManager.GetSimManager();

        gfps.text = fpsv[fpsi].ToString() + " FPS";

        GUILayout.BeginArea(rectDashboard, GUI.skin.box);

        GUILayout.BeginVertical();

        GUILayout.Label(gfps, guiStyleFPS);

        Rect r = GUILayoutUtility.GetLastRect();
        r.x++;
        r.y += r.height + 5;
        r.width -= 2;
        r.height = 100;

        GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, true);

        GUILayout.Space(r.y + r.height);

        if (GUILayout.Button(sm.IsBoundingBoxEnabled() ? "Bounding box ON" : "Bounding box OFF", guiStyleBTN)) {
            sm.SetBoundingBoxEnabled(!sm.IsBoundingBoxEnabled());
            sm.ToggleBoundingBox();
        }

        if (GUILayout.Button(sm.IsDOTSModeEnabled() ? "DOTS mode ON" : "DOTS mode OFF", guiStyleBTN)) {
            sm.SetDOTSModeEnabled(!sm.IsDOTSModeEnabled());
            sm.SwitchMode();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("NS", guiStyleLBL, GUILayout.Width(30));
        sNS = GUILayout.TextField(sNS, guiStyleTF);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("U0", guiStyleLBL, GUILayout.Width(30));
        sU0 = GUILayout.TextField(sU0, guiStyleTF);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("FF", guiStyleLBL, GUILayout.Width(30));
        sFF = GUILayout.TextField(sFF, guiStyleTF);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("SE", guiStyleLBL, GUILayout.Width(30));
        sSE = GUILayout.TextField(sSE, guiStyleTF);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("BS", guiStyleLBL, GUILayout.Width(30));
        sBS = GUILayout.TextField(sBS, guiStyleTF);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset", guiStyleBTN)) {
            sm.Reset();
        }

        GUILayout.EndVertical();

        GUILayout.EndArea();

        if (GUI.changed) {

            try {
                if (sNS.Length == 0) {
                    sNS = "0";
                }
                sm.SetSpawnCount(int.Parse(sNS));
            }
            catch (FormatException ex) {
                sNS = sm.GetSpawnCount().ToString();
                Debug.Log(ex);
            }

            try {
                if (sU0.Length == 0) {
                    sU0 = "0";
                }
                sm.SetInitialSpeed(float.Parse(sU0));
            }
            catch (FormatException ex) {
                sU0 = sm.GetInitialSpeed().ToString();
                Debug.Log(ex);
            }

            try {
                if (sFF.Length == 0) {
                    sFF = "0";
                }
                sm.SetForceFactor(float.Parse(sFF));
            }
            catch (FormatException ex) {
                sFF = sm.GetForceFactor().ToString();
                Debug.Log(ex);
            }

            try {
                if (sSE.Length == 0) {
                    sSE = "0";
                }
                sm.SetSpawnExtent(float.Parse(sSE));
            }
            catch (FormatException ex) {
                sSE = sm.GetSpawnExtent().ToString();
                Debug.Log(ex);
            }

            try {
                if (sBS.Length == 0) {
                    sBS = "0";
                }
                sm.SetBallScale(float.Parse(sBS));
            }
            catch (FormatException ex) {
                sBS = sm.GetBallScale().ToString();
                Debug.Log(ex);
            }
        }

    }

    private void RebuildFPSTex() {

        NativeArray<byte> btex = tex.GetRawTextureData<byte>();

        float d = 255 / tex.height;
        float o = 255 - FPS_CHART_COLOR_THRESHOLD;

        for (int x = 0; x < fpsv.Length; x++) {

            int bx = x - fpsi - 1;
            if (x <= fpsi) {
                bx += fpsv.Length;
            }

            for (int y = 0; y != tex.height; y++) {

                int i = (bx + y * tex.width) * 4;

                if (y > fpsv[x]) {
                    btex[i + 0] = 127;
                    btex[i + 1] = 0;
                    btex[i + 2] = 0;
                    btex[i + 3] = 0;
                }
                else {
                    float c = y * d;
                    btex[i + 0] = 255;
                    // btex[i + 1] = 0;
                    btex[i + 1] = (byte)(Math.Max(0, o - c));
                    // btex[i + 2] = 255;
                    btex[i + 2] = (byte)(c);
                    btex[i + 3] = 0;
                }
            }
        }

        tex.Apply();
    }

    private void UpdateFPS(object sender, ElapsedEventArgs e) {

        if (++fpsi == fpsv.Length) {
            fpsi = 0;
        }

        fpsv[fpsi] = frames * 2;

        frames = 0;

        tfps = DateTime.Now.Ticks;
    }
}
