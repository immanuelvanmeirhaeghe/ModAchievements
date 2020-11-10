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

        private static readonly string s_ModName = nameof(ModAchievements);

        private bool ShowUI = false;

        public bool IsModActiveForMultiplayer { get; private set; }
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public static bool IsDebugShown { get; private set; }
        public string UnLockedAchievementsInfo { get; private set; }

        public static Rect ModModAchievementsScreen = new Rect(Screen.width / 40f, Screen.height / 40f, 750f, 250f);
        public static Vector2 ScrollPosition;
        public static string SelectedAchievementName;
        public static int SelectedAchievementIndex;
        public static AchievementData SelectedAchievementData;

        public static List<string> s_AchievementsDebugData;
        public static Dictionary<string, bool> s_Achievements = new Dictionary<string, bool>();
        public static List<AchievementData> s_AchievementDataList = new List<AchievementData>();


        public static string PermissionChangedMessage(string permission) => $"Permission to use mods and cheats in multiplayer was {permission}";

        private static string HUDBigInfoMessage(string message) => $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>System</color>\n{message}";

        public static string OnlyForSinglePlayerOrHostMessage(string message)
           => $"\n<color=#{ColorUtility.ToHtmlStringRGBA(Color.yellow)}>{message}</color> is only available for single player or when host.\nHost can activate using <b>ModManager</b>.";

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
                    UnLockedAchievementsScrollView();

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
                        LoadAchievementsData();
                    }
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(OnClickLoadAchievementsButton)}] throws exception:\n{exc.Message}");
            }
        }

        private void LoadAchievementsData()
        {
            try
            {
                s_Achievements.Clear();
                s_AchievementDataList.Clear();

                foreach (string s_AchievementDebugData in s_AchievementsDebugData)
                {
                    bool m_IsAchieved = s_AchievementDebugData.Contains("green") ? true : false;
                    string m_ApiName = s_AchievementDebugData.Split('>')[1].Split('<')[0];
                    s_Achievements.Add(m_ApiName, m_IsAchieved);

                    AchievementData achievementData = new AchievementData(m_ApiName);
                    achievementData.SetAchived(m_IsAchieved);
                    s_AchievementDataList.Add(achievementData);
                }

                _ = GetUnLockedAchievementNames();

                s_AchievementDataLoaded = true;
                ShowHUDBigInfo($"Data loaded!");
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

        private void UnLockedAchievementsScrollView()
        {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));
            UnLockedAchievementsInfo = GUILayout.TextArea(UnLockedAchievementsInfo, GUI.skin.textArea);
            GUILayout.EndScrollView();

            if (GUILayout.Button("Toggle debug menu", GUI.skin.button))
            {
                ToggleMenuDebugAchievements();
            }
        }

        private void LockedAchievementsScrollView()
        {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));
            SelectedAchievementIndex = GUILayout.SelectionGrid(SelectedAchievementIndex, GetLockedAchievementNames(), 3, GUI.skin.button);
            GUILayout.EndScrollView();

            if (GUILayout.Button("Unlock achievement", GUI.skin.button))
            {
                OnClickUnlockAchievementButton();
                CloseWindow();
            }
        }

        private void OnClickUnlockAchievementButton()
        {
            try
            {
                string[] achievementNames = GetLockedAchievementNames();
                SelectedAchievementName = achievementNames[SelectedAchievementIndex];
                SelectedAchievementData = s_AchievementDataList.Find(achievement => achievement.GetApiName() == SelectedAchievementName);
                if (SelectedAchievementData != null)
                {
                    //s_AchievementsManager.UnlockAchievement(SelectedAchievementData.GetApiName());
                    ShowHUDBigInfo($"Achievement {SelectedAchievementData.GetApiName()} unlocked!");
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(OnClickUnlockAchievementButton)}] throws exception:\n{exc.Message}");
            }
        }

        private string[] GetLockedAchievementNames()
        {
            List<string> list = new List<string>();
            try
            {
                if (s_Achievements != null)
                {
                    var lockedAchievements = s_Achievements.Where(ach => ach.Value == false).ToArray();

                    foreach (var lockedAchievement in lockedAchievements)
                    {
                        list.Add(lockedAchievement.Key);
                    }
                }
                return list.ToArray();
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(GetLockedAchievementNames)}] throws exception:\n{exc.Message}");
                return list.ToArray();
            }
        }

        private string[] GetUnLockedAchievementNames()
        {
            List<string> list = new List<string>();
            try
            {
                if (s_Achievements != null)
                {
                    var unlockedAchievements = s_Achievements.Where(ach => ach.Value == true).ToArray();

                    foreach (var unlockedAchievement in unlockedAchievements)
                    {
                        UnLockedAchievementsInfo += unlockedAchievement.Key;
                        list.Add(unlockedAchievement.Key);
                    }
                }
                return list.ToArray();
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{s_ModName}:{nameof(GetLockedAchievementNames)}] throws exception:\n{exc.Message}");
                return list.ToArray();
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

        public void Start()
        {
            ModManager.ModManager.onPermissionValueChanged += ModManager_onPermissionValueChanged;
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
    }
}
