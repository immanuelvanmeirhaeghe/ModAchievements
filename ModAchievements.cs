using ModAchievements.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModAchievements
{
    public class ModAchievements : MonoBehaviour
    {
        private static ModAchievements s_Instance;
        private static HUDManager s_HUDManager;
        private static MenuInGameManager s_MenuInGameManager;
        private static AchievementsManager s_AchievementsManager;
        private static Player s_Player;
        private static bool s_AchievementDataLoaded = false;
        private static MenuDebugAchievements s_MenuDebugAchievements;
        private static StringBuilder DebugLogger = new StringBuilder($"");
        private static readonly string s_ModName = nameof(ModAchievements);
        private static Color DefaultColor;

        private bool ShowUI = false;

        public bool IsModActiveForMultiplayer { get; private set; }
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public static bool IsDebugShown { get; private set; }
       // public string UnlockedAchievementsInfo { get; private set; }

        public static Rect ModModAchievementsScreen = new Rect(Screen.width / 40f, Screen.height / 40f, 750f, 250f);
        public static Vector2 UnlockedAchievementsScrollViewPosition;
        public static Vector2 LockedAchievementsScrollViewPosition;
        public static string SelectedAchievementTitle;
        public static int SelectedAchievementTitleIndex;
        public static AchievementData SelectedAchievementData;

        public static AchievementsManager GetManager() => s_AchievementsManager;
        public static List<string> s_AchievementsDebugData = new List<string>();
        public static Dictionary<string, bool> s_Achievements = new Dictionary<string, bool>();
        public static List<AchievementData> s_AchievementDataList = new List<AchievementData>();
        public static List<AchievementInfo> s_AchievementsInfo = new List<AchievementInfo>();

        public static string PermissionChangedMessage(string message) => $"Permission to use mods and cheats in multiplayer was {message}";

        private static string HUDBigInfoMessage(string message) => $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>System</color>\n{message}";

        public static string OnlyForSinglePlayerOrHostMessage()
           => $"Only available for single player or when host.\nHost can activate using ModManager.";

        public void ShowHUDBigInfo(string text)
        {
            string header = $"{s_ModName} Info";
            string textureName = HUDInfoLogTextureType.Count.ToString();

            HUDBigInfo bigInfo = (HUDBigInfo)s_HUDManager.GetHUD(typeof(HUDBigInfo));
            HUDBigInfoData.s_Duration = 4f;
            HUDBigInfoData bigInfoData = new HUDBigInfoData
            {
                m_Header = header,
                m_Text = text,
                m_TextureName = textureName,
                m_ShowTime = Time.time
            };
            bigInfo.AddInfo(bigInfoData);
            bigInfo.Show(true);
        }

        private void ToggleShowUI()
        {
            ShowUI = !ShowUI;
        }

        public void Start()
        {
            ModManager.ModManager.onPermissionValueChanged += ModManager_onPermissionValueChanged;
            DefaultColor = GUI.contentColor;
        }

        private void ModManager_onPermissionValueChanged(bool optionValue)
        {
            IsModActiveForMultiplayer = optionValue;
            ShowHUDBigInfo(
                          (optionValue ?
                            HUDBigInfoMessage(PermissionChangedMessage($"<color=#{ColorUtility.ToHtmlStringRGBA(Color.green)}>granted!</color>"))
                            : HUDBigInfoMessage(PermissionChangedMessage($"<color=#{ColorUtility.ToHtmlStringRGBA(Color.yellow)}>revoked!</color>")))
                            );
        }

        public ModAchievements()
        {
            useGUILayout = true;
            s_Instance = this;
        }

        public static ModAchievements Get()
        {
            return s_Instance;
        }

        public void ShowHUDInfoLog(string itemID, string localizedTextKey)
        {
            var localization = GreenHellGame.Instance.GetLocalization();
            HUDMessages hUDMessages = (HUDMessages)s_HUDManager.GetHUD(typeof(HUDMessages));
            hUDMessages.AddMessage(
                $"{localization.Get(localizedTextKey)}  {localization.Get(itemID)}"
                );
        }

        private void EnableCursor(bool blockPlayer = false)
        {
            CursorManager.Get().ShowCursor(blockPlayer, false);

            if (blockPlayer)
            {
                s_Player.BlockMoves();
                s_Player.BlockRotation();
                s_Player.BlockInspection();
            }
            else
            {
                s_Player.UnblockMoves();
                s_Player.UnblockRotation();
                s_Player.UnblockInspection();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                if (!ShowUI)
                {
                    InitData();
                    EnableCursor(true);
                }
                ToggleShowUI();
                if (!ShowUI)
                {
                    EnableCursor(false);
                }
            }
        }

        private void OnGUI()
        {
            if (ShowUI)
            {
                InitData();
                InitSkinUI();
                InitWindow();
            }
        }

        private void InitWindow()
        {
            int wid = GetHashCode();
            ModModAchievementsScreen = GUILayout.Window(wid, ModModAchievementsScreen, InitModAchievementsScreen, $"{s_ModName}", GUI.skin.window);
        }

        private void InitData()
        {
            s_HUDManager = HUDManager.Get();
            s_Player = Player.Get();
            s_MenuInGameManager = MenuInGameManager.Get();
            s_AchievementsManager = AchievementsManager.Get();
        }

        private void InitSkinUI()
        {
            GUI.skin = ModAPI.Interface.Skin;
        }

        private void InitModAchievementsScreen(int windowID)
        {
            using (var verticalScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                ScreenMenuBox();

                AchievementsBox();
            }
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
        }

        private void AchievementsBox()
        {
            using (var verScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                if (GUILayout.Button("Load achievements", GUI.skin.button))
                {
                    OnClickLoadAchievementsButton();
                }

                if (s_AchievementDataLoaded)
                {
                    UnlockedAchievementsScrollView();

                    LockedAchievementsScrollView();
                }
            }
        }

        private void OnClickLoadAchievementsButton()
        {
            try
            {
                if (s_AchievementsManager != null)
                {
                    s_AchievementsDebugData = s_AchievementsManager.GetAchievementsDebugData();
                    if (s_AchievementsDebugData != null && s_AchievementsDebugData.Count > 0)
                    {
                        LoadAchievementsData(true);
                    }
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(OnClickLoadAchievementsButton)}] throws exception:\n{exc.Message}");
            }
        }

        private void LoadAchievementsData(bool debug = false)
        {
            try
            {
                s_Achievements.Clear();
                s_AchievementDataList.Clear();
                s_AchievementsInfo.Clear();
                if (debug)
                {
                    DebugLogger.Clear();
                    DebugLogger.AppendLine($"Achievements");
                }

                if (s_AchievementsDebugData == null || s_AchievementsDebugData.Count == 0)
                {
                    if (debug)
                    {
                        DebugLogger.AppendLine($"\nData not found!");
                        ModAPI.Log.Write(DebugLogger.ToString());
                    }
                    return;
                }

                foreach (string s_AchievementDebugData in s_AchievementsDebugData)
                {
                    bool m_IsAchieved = s_AchievementDebugData.Contains("green") ? true : false;
                    string m_ApiName = s_AchievementDebugData.Split('>')[1].Split('<')[0];
                    s_Achievements.Add(m_ApiName, m_IsAchieved);

                    AchievementData achievementData = new AchievementData(m_ApiName);
                    achievementData.SetAchived(m_IsAchieved);
                    s_AchievementDataList.Add(achievementData);

                    AchievementInfo achievementInfo = new AchievementInfo(m_ApiName, achievementData);
                    s_AchievementsInfo.Add(achievementInfo);

                    if (debug)
                    {
                        DebugLogger.AppendLine($"\n{achievementInfo.AchievementID}\t\t\t\t{achievementInfo.Title}\t\t\t\t{achievementInfo.AchievementData.IsAchieved()}");
                    }
                }

                if (debug)
                {
                    ModAPI.Log.Write(DebugLogger.ToString());
                }

                s_AchievementDataLoaded = true;
                ShowHUDBigInfo(HUDBigInfoMessage($"Data loaded!"));
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(LoadAchievementsData)}] throws exception:\n{exc.Message}");
                s_AchievementDataLoaded = false;
            }
        }

        private static void ToggleMenuDebugAchievements()
        {
            if (IsDebugShown && s_MenuDebugAchievements != null)
            {
                s_MenuDebugAchievements.Hide();
                IsDebugShown = false;
            }
            else
            {
                s_MenuDebugAchievements = (MenuDebugAchievements)s_MenuInGameManager.GetMenu(typeof(MenuDebugAchievements));
                if (s_MenuDebugAchievements != null)
                {
                    s_MenuDebugAchievements.Show();
                    IsDebugShown = true;
                }
            }
        }

        private void UnlockedAchievementsScrollView()
        {
            UnlockedAchievementsScrollViewPosition = GUILayout.BeginScrollView(UnlockedAchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));
            //GUILayout.Label(UnlockedAchievementsInfo, GUI.skin.label);
            if (s_AchievementsInfo != null && s_AchievementsInfo.Count > 0)
            {
                GUI.contentColor = Color.green;
                foreach (AchievementInfo unlockedAchievementInfo in s_AchievementsInfo.Where(achievementInfo => achievementInfo.AchievementData.IsAchieved()))
                {
                    using (var horScope = new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        GUILayout.Box(unlockedAchievementInfo.IconImage, GUI.skin.box, GUILayout.MaxWidth(65f), GUILayout.MaxHeight(65f));
                        GUILayout.Label(unlockedAchievementInfo.Title, GUI.skin.label);
                        GUILayout.Label(unlockedAchievementInfo.Description, GUI.skin.label);
                    }
                }
            }
            GUILayout.EndScrollView();

            GUI.contentColor = DefaultColor;
            if (IsModActiveForMultiplayer)
            {
                if (GUILayout.Button("Toggle debug menu", GUI.skin.button))
                {
                    ToggleMenuDebugAchievements();
                }
            }
            else
            {
                GUILayout.Label(OnlyForSinglePlayerOrHostMessage(), GUI.skin.label);
            }
        }

        private void LockedAchievementsScrollView()
        {
            LockedAchievementsScrollViewPosition = GUILayout.BeginScrollView(LockedAchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));
            //GUI.contentColor = Color.red;
            //SelectedAchievementIndex = GUILayout.SelectionGrid(SelectedAchievementIndex, GetAchievementNames(true), 3, GUI.skin.button);
            if (s_AchievementsInfo != null && s_AchievementsInfo.Count > 0)
            {
                GUI.contentColor = Color.red;
                foreach (AchievementInfo lockedAchievementInfo in s_AchievementsInfo.Where(achievementInfo => !achievementInfo.AchievementData.IsAchieved()))
                {
                    using (var horScope = new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        GUILayout.Box(lockedAchievementInfo.IconImage, GUI.skin.box, GUILayout.MaxWidth(65f), GUILayout.MaxHeight(65f));
                        GUILayout.Label(lockedAchievementInfo.Title, GUI.skin.label);
                        GUILayout.Label(lockedAchievementInfo.Description, GUI.skin.label);
                        GUI.contentColor = DefaultColor;
                        if (GUILayout.Button("Unlock", GUI.skin.button))
                        {
                            OnClickUnlockAchievementButton(lockedAchievementInfo);
                            CloseWindow();
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        private void OnClickUnlockAchievementButton(AchievementInfo achievementInfo)
        {
            try
            {
                //string[] lockedAchievementTitles = GetAchievementTitles(false);
                //SelectedAchievementTitle = lockedAchievementTitles[SelectedAchievementTitleIndex];
                SelectedAchievementData = achievementInfo.AchievementData;
                if (SelectedAchievementData != null)
                {
                    //s_AchievementsManager.UnlockAchievement(SelectedAchievementData.GetApiName());
                    ShowHUDBigInfo(HUDBigInfoMessage($"Achievement {SelectedAchievementTitle} unlocked!"));
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(OnClickUnlockAchievementButton)}] throws exception:\n{exc.Message}");
            }
        }

        private string[] GetAchievementTitles(bool isAchieved)
        {
            List<string> achievementNames = new List<string>();
            try
            {
                if (s_AchievementsInfo != null)
                {
                    var achievements = s_AchievementsInfo.Where(ach => ach.AchievementData.IsAchieved() == isAchieved).ToArray();
                    if (achievements != null)
                    {
                        foreach (var achievement in achievements)
                        {
                            achievementNames.Add(achievement.Title);
                        }
                    }
                }
                return achievementNames.ToArray();
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(GetAchievementTitles)}({nameof(isAchieved)}={isAchieved})] throws exception:\n{exc.Message}");
                return achievementNames.ToArray();
            }
        }

        private void ScreenMenuBox()
        {
            if (GUI.Button(new Rect(ModModAchievementsScreen.width - 20f, 0f, 20f, 20f), "X", GUI.skin.button))
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            ShowUI = false;
            EnableCursor(false);
        }
    }
}
