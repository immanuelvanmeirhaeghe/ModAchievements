using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModAchievements
{
    /// <summary>
    /// ModAchievements is a mod for Green Hell, that allows a player to manage Steam achievements.
    /// Enable the mod UI by pressing Home.
    /// </summary>
    public class ModAchievements : MonoBehaviour
    {
        private static readonly string ModName = nameof(ModAchievements);
        private static readonly StringBuilder AchievementInfoLogger = new StringBuilder($"{ModName} debug info");
        private static readonly float ModScreenTotalWidth = 500f;
        private static readonly float ModScreenTotalHeight = 150f;
        private static readonly float ModScreenMinWidth = 450f;
        private static readonly float ModScreenMaxWidth = 550f;
        private static readonly float ModScreenMinHeight = 50f;
        private static readonly float ModScreenMaxHeight = 200f;

        private static ModAchievements Instance;
        private static HUDManager LocalHUDManager;
        private static MenuInGameManager LocalMenuInGameManager;
        private static AchievementsManager LocalAchievementsManager;
        private static Player LocalPlayer;
        private static MenuDebugAchievements LocalMenuDebugAchievements;
        private static Color LocalDefaultColor;

        private static float ModScreenStartPositionX { get; set; } = (Screen.width - ModScreenMaxWidth) % ModScreenTotalWidth;
        private static float ModScreenStartPositionY { get; set; } = (Screen.height - ModScreenMaxHeight) % ModScreenTotalHeight;
        private static bool IsMinimized { get; set; } = false;
        private bool ShowModUI = false;

        public bool IsModActiveForMultiplayer { get; private set; } = false;
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public static bool AchievementDataLoaded { get; set; } = false;
        public static bool LogAchievementInfoOption { get; private set; } = false;
        public static bool IsLocalMenuDebugAchievementsShown { get; private set; } = false;

        public static Rect LocalDebugMenuAchievementsScreen = new Rect(ModAchievementsScreen.x, ModAchievementsScreen.y, ModAchievementsScreen.width / 2f, ModAchievementsScreen.height / 2f);
        public static Rect ModAchievementsScreen = new Rect(ModScreenStartPositionX, ModScreenStartPositionY, ModScreenTotalWidth, ModScreenTotalHeight);
        public static Vector2 UnlockedAchievementsScrollViewPosition;
        public static Vector2 LockedAchievementsScrollViewPosition;

        public static AchievementData SelectedAchievementData;
        public static List<string> LocalAchievementsDebugData = new List<string>();
        public static List<AchievementData> LocalAchievementDataList = new List<AchievementData>();
        public static List<AchievementInfo> LocalAchievementsInfoList = new List<AchievementInfo>();

        public static string HUDBigInfoMessage(string message)
            => $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>System</color>\n{message}";
        public static string OnlyInDebugModeMessage()
           => $"Only available in debug mode.\nHost can activate using ModManager. or DebugMode";
        public static string PermissionChangedMessage(string message)
            => $"Permission to use mods and cheats in multiplayer was {message}";

        public void ShowHUDBigInfo(string text)
        {
            string header = $"{ModName} Info";
            string textureName = HUDInfoLogTextureType.Count.ToString();

            HUDBigInfo bigInfo = (HUDBigInfo)LocalHUDManager.GetHUD(typeof(HUDBigInfo));
            HUDBigInfoData.s_Duration = 2f;
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
            ShowModUI = !ShowModUI;
        }

        public void Start()
        {
            ModManager.ModManager.onPermissionValueChanged += ModManager_onPermissionValueChanged;
            LocalDefaultColor = GUI.contentColor;
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
            Instance = this;
        }

        public static ModAchievements Get()
        {
            return Instance;
        }

        private void EnableCursor(bool blockPlayer = false)
        {
            CursorManager.Get().ShowCursor(blockPlayer, false);

            if (blockPlayer)
            {
                LocalPlayer.BlockMoves();
                LocalPlayer.BlockRotation();
                LocalPlayer.BlockInspection();
            }
            else
            {
                LocalPlayer.UnblockMoves();
                LocalPlayer.UnblockRotation();
                LocalPlayer.UnblockInspection();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                if (!ShowModUI)
                {
                    InitData();
                    EnableCursor(true);
                }
                ToggleShowUI();
                if (!ShowModUI)
                {
                    EnableCursor(false);
                }
            }
        }

        private void OnGUI()
        {
            if (ShowModUI)
            {
                InitData();
                InitSkinUI();
                InitWindow();
            }
        }

        private void InitWindow()
        {
            int wid = GetHashCode();
            ModAchievementsScreen = GUILayout.Window(wid, ModAchievementsScreen, InitModAchievementsScreen, ModName, GUI.skin.window,
                                                                                                          GUILayout.ExpandWidth(true),
                                                                                                          GUILayout.MinWidth(ModScreenMinWidth),
                                                                                                          GUILayout.MaxWidth(ModScreenMaxWidth),
                                                                                                          GUILayout.ExpandHeight(true),
                                                                                                          GUILayout.MinHeight(ModScreenMinHeight),
                                                                                                          GUILayout.MaxHeight(ModScreenMaxHeight));
        }

        private void InitData()
        {
            LocalHUDManager = HUDManager.Get();
            LocalPlayer = Player.Get();
            LocalMenuInGameManager = MenuInGameManager.Get();
            LocalAchievementsManager = AchievementsManager.Get();
        }

        private void InitSkinUI()
        {
            GUI.skin = ModAPI.Interface.Skin;
        }

        private void InitModAchievementsScreen(int windowID)
        {
            ModScreenStartPositionX = ModAchievementsScreen.x;
            ModScreenStartPositionY = ModAchievementsScreen.y;

            using (var modContentScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                ScreenMenuBox();
                if (!IsMinimized)
                {
                    ModOptionsBox();
                    AchievementsBox();
                }
            }
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
        }

        private void AchievementsBox()
        {
            using (var verScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                GUI.contentColor = LocalDefaultColor;
                if (IsModActiveForSingleplayer || IsModActiveForMultiplayer)
                {
                    if (GUILayout.Button("Toggle statistics info", GUI.skin.button))
                    {
                        ToggleDebugStatistics();
                    }
                }
                else
                {
                    GUILayout.Label(OnlyInDebugModeMessage(), GUI.skin.label);
                }

                if (GUILayout.Button("Load achievements", GUI.skin.button))
                {
                    OnClickLoadAchievementsButton();
                }

                if (AchievementDataLoaded)
                {
                    UnlockedAchievementsScrollView();

                    LockedAchievementsScrollView();
                }
            }
        }

        private void ModOptionsBox()
        {
            using (var optionScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                if (IsModActiveForSingleplayer || IsModActiveForMultiplayer)
                {
                    GUI.color = Color.green;
                    GUILayout.Label(PermissionChangedMessage($"granted"), GUI.skin.label);
                }
                else
                {
                    GUI.color = Color.yellow;
                    PermissionChangedMessage($"revoked");
                }
                GUI.color = Color.white;
                LogAchievementInfoOption = GUILayout.Toggle(LogAchievementInfoOption, $"Log achievement info?", GUI.skin.toggle);
            }
        }

        private void OnClickLoadAchievementsButton()
        {
            try
            {
                if (LocalAchievementsManager != null)
                {
                    LocalAchievementsDebugData = LocalAchievementsManager.GetAchievementsDebugData();
                    if (LocalAchievementsDebugData != null && LocalAchievementsDebugData.Count > 0)
                    {
                        LoadAchievementsData(LogAchievementInfoOption);
                    }
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{ModName}:{nameof(OnClickLoadAchievementsButton)}] throws exception:\n{exc.Message}");
            }
        }

        private void LoadAchievementsData(bool logInfo = false)
        {
            try
            {
                LocalAchievementDataList.Clear();
                LocalAchievementsInfoList.Clear();
                AchievementInfoLogger.Clear();

                if (LocalAchievementsDebugData == null || LocalAchievementsDebugData.Count == 0)
                {
                    if (logInfo)
                    {
                        AchievementInfoLogger.AppendLine($"{nameof(LocalAchievementsDebugData)} not found!");
                        ModAPI.Log.Write(AchievementInfoLogger.ToString());
                    }
                    return;
                }

                if (logInfo)
                {
                    AchievementInfoLogger.AppendLine($"{nameof(AchievementInfo.AchievementID)}\t\t\t\t\t\t\t\t\t{nameof(AchievementInfo.AchievementTitle)}\t\t\t\t\t\t\t\t\t" +
                        $"{nameof(AchievementInfo.AchievementData.IsAchieved)}\t\t\t\t\t\t\t\t\t{nameof(AchievementInfo.AchievementIconFileUri)}");
                }

                foreach (string achievementDebugData in LocalAchievementsDebugData)
                {
                    ///  achievementDebugData should have output formatted as "<color=" + (m_IsAchieved ? "green" : "red") + ">" + m_ApiName + "</color>"
                    bool isAchieved = achievementDebugData.Contains("green") ? true : false;
                    string apiName = achievementDebugData.Split('>')[1]?.Split('<')[0];

                    AchievementData achievementData = new AchievementData(apiName);
                    achievementData.SetAchived(isAchieved);
                    LocalAchievementDataList.Add(achievementData);

                    AchievementInfo achievementInfo = new AchievementInfo(apiName, achievementData);
                    if (achievementInfo != null)
                    {
                        StartCoroutine(achievementInfo.StartGetTexture(achievementInfo.AchievementIconFileUri.ToString()));
                    }
                    LocalAchievementsInfoList.Add(achievementInfo);

                    if (logInfo)
                    {
                        AchievementInfoLogger.AppendLine($"{achievementInfo.AchievementID}\t\t\t\t\t\t\t\t\t{achievementInfo.AchievementTitle}\t\t\t\t\t\t\t\t\t" +
                                                                         $"{achievementInfo.AchievementData.IsAchieved()}\t\t\t\t\t\t\t\t\t{achievementInfo.AchievementIconFileUri}");
                    }
                }

                if (logInfo)
                {
                    ModAPI.Log.Write(AchievementInfoLogger.ToString());
                }

                AchievementDataLoaded = true;
            }
            catch (Exception exc)
            {
                AchievementInfoLogger.AppendLine($"[{ModName}:{nameof(LoadAchievementsData)}({nameof(logInfo)} = {logInfo}] throws exception:\n{exc.Message}");
                ModAPI.Log.Write(AchievementInfoLogger.ToString());
                AchievementDataLoaded = false;
            }
        }

        private void ToggleDebugStatistics()
        {
            if (IsLocalMenuDebugAchievementsShown && LocalMenuDebugAchievements != null)
            {
                LocalDebugMenuAchievementsScreen = default;

                LocalMenuDebugAchievements.Hide();
                IsLocalMenuDebugAchievementsShown = false;
            }
            else
            {
                LocalMenuDebugAchievements = (MenuDebugAchievements)LocalMenuInGameManager.GetMenu(typeof(MenuDebugAchievements));
                if (LocalMenuDebugAchievements != null)
                {
                    ShowDebugMenuAchievements();
                    IsLocalMenuDebugAchievementsShown = true;
                }
            }
        }

        private void ShowDebugMenuAchievements()
        {
            int wid = GetHashCode();
            LocalDebugMenuAchievementsScreen = GUILayout.Window(wid, ModAchievementsScreen, InitDebugMenuAchievementsScreen, nameof(MenuDebugAchievements), GUI.skin.window,
                                                                                                  GUILayout.ExpandWidth(true),
                                                                                                  GUILayout.MinWidth(ModScreenMinWidth / 2f),
                                                                                                  GUILayout.MaxWidth(ModScreenMaxWidth / 2f),
                                                                                                  GUILayout.ExpandHeight(true),
                                                                                                  GUILayout.MinHeight(ModScreenMinHeight / 2f),
                                                                                                  GUILayout.MaxHeight(ModScreenMaxHeight / 2f));
        }

        private void InitDebugMenuAchievementsScreen(int id)
        {
            LocalMenuDebugAchievements.Show();
        }

        private void UnlockedAchievementsScrollView()
        {
            UnlockedAchievementsScrollViewPosition = GUILayout.BeginScrollView(UnlockedAchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));

            if (LocalAchievementsInfoList != null && LocalAchievementsInfoList.Count > 0)
            {
                AddAchievementInfoLabels(true);
            }

            GUILayout.EndScrollView();
        }

        private void LockedAchievementsScrollView()
        {
            LockedAchievementsScrollViewPosition = GUILayout.BeginScrollView(LockedAchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));

            if (LocalAchievementsInfoList != null && LocalAchievementsInfoList.Count > 0)
            {
                AddAchievementInfoLabels(false);
            }

            GUILayout.EndScrollView();
        }

        private void AddAchievementInfoLabels(bool isAchieved)
        {
            try
            {
                var localAchievementsInfoList = GetAchievementInfo(isAchieved);
                if (localAchievementsInfoList != null && localAchievementsInfoList.Count() > 0)
                {
                    foreach (AchievementInfo localAchievementInfo in localAchievementsInfoList)
                    {
                        GUI.contentColor = isAchieved ? Color.green : Color.red;
                        GUIContent content = new GUIContent(localAchievementInfo.AchievementTitle, localAchievementInfo.AchievementIconTexture, localAchievementInfo.AchievementDescription);
                        using (var horScope = new GUILayout.HorizontalScope(GUI.skin.box))
                        {
                            GUILayout.Label(content, GUI.skin.label);

                            if (!isAchieved)
                            {
                                GUI.contentColor = LocalDefaultColor;
                                if (GUILayout.Button("Unlock", GUI.skin.button))
                                {
                                    SelectedAchievementData = localAchievementInfo.AchievementData;
                                    OnClickUnlockAchievementButton();
                                    CloseWindow();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{ModName}:{nameof(GetAchievementInfo)}({nameof(isAchieved)}={isAchieved})] throws exception:\n{exc.Message}");
            }
        }

        private void OnClickUnlockAchievementButton()
        {
            try
            {
                if (SelectedAchievementData != null && !SelectedAchievementData.IsAchieved())
                {
                    LocalAchievementsManager.UnlockAchievement(SelectedAchievementData.GetApiName());
                    ShowHUDBigInfo(HUDBigInfoMessage($"Achievement {SelectedAchievementData.GetApiName()} unlocked!"));
                }
                else
                {
                    ShowHUDBigInfo(HUDBigInfoMessage($"Achievement is already unlocked!"));
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{ModName}:{nameof(OnClickUnlockAchievementButton)}] throws exception:\n{exc.Message}");
            }
        }

        private AchievementInfo[] GetAchievementInfo(bool isAchieved)
        {
            AchievementInfo[] achievementInfoArray = default;
            try
            {
                if (LocalAchievementsInfoList != null && LocalAchievementsInfoList.Count > 0)
                {
                    achievementInfoArray = LocalAchievementsInfoList.Where(achievementInfo => achievementInfo.AchievementData.IsAchieved() == isAchieved)?.ToArray();
                }
                return achievementInfoArray;
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{ModName}:{nameof(GetAchievementInfo)}({nameof(isAchieved)}={isAchieved})] throws exception:\n{exc.Message}");
                return achievementInfoArray;
            }
        }

        private void ScreenMenuBox()
        {
            if (GUI.Button(new Rect(ModAchievementsScreen.width - 40f, 0f, 20f, 20f), "-", GUI.skin.button))
            {
                CollapseWindow();
            }

            if (GUI.Button(new Rect(ModAchievementsScreen.width - 20f, 0f, 20f, 20f), "X", GUI.skin.button))
            {
                CloseWindow();
            }
        }

        private void CollapseWindow()
        {
            if (!IsMinimized)
            {
                ModAchievementsScreen = new Rect(ModScreenStartPositionX, ModScreenStartPositionY, ModScreenTotalWidth, ModScreenMinHeight);
                IsMinimized = true;
            }
            else
            {
                ModAchievementsScreen = new Rect(ModScreenStartPositionX, ModScreenStartPositionY, ModScreenTotalWidth, ModScreenTotalHeight);
                IsMinimized = false;
            }
            InitWindow();
        }

        private void CloseWindow()
        {
            ShowModUI = false;
            EnableCursor(false);
        }
    }
}
