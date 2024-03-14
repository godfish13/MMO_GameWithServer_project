using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiPlayerBuildandRun
{
    [MenuItem("Tools/Run MultiPlayer/2 players")]
    static void Run2players()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run MultiPlayer/3 players")]
    static void Run3players()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run MultiPlayer/4 players")]
    static void Run4players()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        // window �������� ����

        for(int i = 0; i < playerCount; i++) 
        {
            BuildPipeline.BuildPlayer(GetScenePath(), 
                "0.TestBuilds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe", 
                BuildTarget.StandaloneWindows64,
                BuildOptions.AutoRunPlayer);
        }   // playerCount ������ŭ ������Ʈ���� ����� �ڵ�����
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    } // �� ������Ʈ�� ��θ� �޾ƿ��� '/'���� �����ؼ� ������� �־���
      // ����� : s[0] = C:, s[1] = Unity_Projects, s[2] = MMO_GameWithServer_Project, s[3] = Client, s[4] = Assets

    static string[] GetScenePath()  // BuildSetting�� �߰��Ǿ��ִ� scene�� �ڵ�� �ܾ����
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path; 
        }
        return scenes;
    }
}
