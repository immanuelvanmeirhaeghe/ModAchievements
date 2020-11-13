using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModAchievements
{
    public class ModAchievements : MonoBehaviour
    {
        private static ModAchievements Instance;
        private static HUDManager LocalHUDManager;
        private static MenuInGameManager LocalMenuInGameManager;
        private static AchievementsManager LocalAchievementsManager;
        private static Player LocalPlayer;
        private static bool AchievementDataLoaded = false;
        private static MenuDebugAchievements LocalMenuDebugAchievements;
        private static readonly StringBuilder DebugLogger = new StringBuilder($"Achievements debug info");
        private static readonly string LocalModName = nameof(ModAchievements);
        private static Color DefaultColor;

        public static bool LogLocalDebugInfoOption { get; private set; }
        public static bool IsLocalMenuDebugAchievementsShown { get; private set; }
        public static Rect ModModAchievementsScreen = new Rect(Screen.width / 40f, Screen.height / 40f, 750f, 450f);
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

        private bool ShowModUI = false;

        public bool IsModActiveForMultiplayer { get; private set; }
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public void ShowHUDBigInfo(string text)
        {
            string header = $"{LocalModName} Info";
            string textureName = HUDInfoLogTextureType.Count.ToString();

            HUDBigInfo bigInfo = (HUDBigInfo)LocalHUDManager.GetHUD(typeof(HUDBigInfo));
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
            ShowModUI = !ShowModUI;
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
            Instance = this;
        }

        public static ModAchievements Get()
        {
            return Instance;
        }

        public void ShowHUDInfoLog(string itemID, string localizedTextKey)
        {
            var localization = GreenHellGame.Instance.GetLocalization();
            HUDMessages hUDMessages = (HUDMessages)LocalHUDManager.GetHUD(typeof(HUDMessages));
            hUDMessages.AddMessage(
                $"{localization.Get(localizedTextKey)}  {localization.Get(itemID)}"
                );
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
            ModModAchievementsScreen = GUILayout.Window(wid, ModModAchievementsScreen, InitModAchievementsScreen, $"{LocalModName}", GUI.skin.window);
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
                LogLocalDebugInfoOption = GUILayout.Toggle(LogLocalDebugInfoOption, $"Log debug info?", GUI.skin.toggle);
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

        private void OnClickLoadAchievementsButton()
        {
            try
            {
                if (LocalAchievementsManager != null)
                {
                    LocalAchievementsDebugData = LocalAchievementsManager.GetAchievementsDebugData();
                    if (LocalAchievementsDebugData != null && LocalAchievementsDebugData.Count > 0)
                    {
                        LoadAchievementsData(LogLocalDebugInfoOption);
                    }
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{LocalModName}:{nameof(OnClickLoadAchievementsButton)}] throws exception:\n{exc.Message}");
            }
        }

        private void LoadAchievementsData(bool logDebugInfo = false)
        {
            try
            {
                LocalAchievementDataList.Clear();
                LocalAchievementsInfoList.Clear();
                DebugLogger.Clear();

                if (LocalAchievementsDebugData == null || LocalAchievementsDebugData.Count == 0)
                {
                    if (logDebugInfo)
                    {
                        DebugLogger.AppendLine($"{nameof(LocalAchievementsDebugData)} not found!");
                        ModAPI.Log.Write(DebugLogger.ToString());
                    }
                    return;
                }

                if (logDebugInfo)
                {
                    DebugLogger.AppendLine($"{nameof(AchievementInfo.AchievementID)}\t\t\t\t\t\t\t\t\t{nameof(AchievementInfo.AchievementTitle)}\t\t\t\t\t\t\t\t\t" +
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

                    if (logDebugInfo)
                    {
                        DebugLogger.AppendLine($"{achievementInfo.AchievementID}\t\t\t\t\t\t\t\t\t{achievementInfo.AchievementTitle}\t\t\t\t\t\t\t\t\t" +
                                                                         $"{achievementInfo.AchievementData.IsAchieved()}\t\t\t\t\t\t\t\t\t{achievementInfo.AchievementIconFileUri}");
                    }
                }

                if (logDebugInfo)
                {
                    ModAPI.Log.Write(DebugLogger.ToString());
                }

                AchievementDataLoaded = true;
            }
            catch (Exception exc)
            {
                DebugLogger.AppendLine($"[{LocalModName}:{nameof(LoadAchievementsData)}({nameof(logDebugInfo)} = {logDebugInfo}] throws exception:\n{exc.Message}");
                ModAPI.Log.Write(DebugLogger.ToString());
                AchievementDataLoaded = false;
            }
        }

        private static void ToggleMenuDebugAchievements()
        {
            if (IsLocalMenuDebugAchievementsShown && LocalMenuDebugAchievements != null)
            {
                LocalMenuDebugAchievements.Hide();
                IsLocalMenuDebugAchievementsShown = false;
            }
            else
            {
                LocalMenuDebugAchievements = (MenuDebugAchievements)LocalMenuInGameManager.GetMenu(typeof(MenuDebugAchievements));
                if (LocalMenuDebugAchievements != null)
                {
                    LocalMenuDebugAchievements.Show();
                    IsLocalMenuDebugAchievementsShown = true;
                }
            }
        }

        private void UnlockedAchievementsScrollView()
        {
            UnlockedAchievementsScrollViewPosition = GUILayout.BeginScrollView(UnlockedAchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));

            if (LocalAchievementsInfoList != null && LocalAchievementsInfoList.Count > 0)
            {
                AddAchievementInfoLabels(true);
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
                GUILayout.Label(OnlyInDebugModeMessage(), GUI.skin.label);
            }
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
                                GUI.contentColor = DefaultColor;
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
                ModAPI.Log.Write($"[{LocalModName}:{nameof(GetAchievementInfo)}({nameof(isAchieved)}={isAchieved})] throws exception:\n{exc.Message}");
            }
        }

        private void OnClickUnlockAchievementButton()
        {
            try
            {
                if (SelectedAchievementData != null && !SelectedAchievementData.IsAchieved())
                {
                    //s_AchievementsManager.UnlockAchievement(SelectedAchievementData.GetApiName());
                    ShowHUDBigInfo(HUDBigInfoMessage($"Achievement {SelectedAchievementData.GetApiName()} unlocked!"));
                }
                else
                {
                    ShowHUDBigInfo(HUDBigInfoMessage($"Achievement is already unlocked!"));
                }
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{LocalModName}:{nameof(OnClickUnlockAchievementButton)}] throws exception:\n{exc.Message}");
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
                ModAPI.Log.Write($"[{LocalModName}:{nameof(GetAchievementInfo)}({nameof(isAchieved)}={isAchieved})] throws exception:\n{exc.Message}");
                return achievementInfoArray;
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
            ShowModUI = false;
            EnableCursor(false);
        }
    }
}
