using ModAchievements.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace ModAchievements
{
    /// <summary>
    /// ModAchievements is a mod for Green Hell that allows a player to manage Steam achievements.
    /// Press Alpha9 (default) or the key configurable in ModAPI to open the mod screen.
    /// </summary>
    public class ModAchievements : MonoBehaviour
    {
        private static readonly string ModName = nameof(ModAchievements);
        private static readonly StringBuilder AchievementInfoLogger = new StringBuilder($"{ModName} debug info");
        private static readonly float ModScreenTotalWidth = 850f;
        private static readonly float ModScreenTotalHeight = 500f;
        private static readonly float ModScreenMinWidth = 800f;
        private static readonly float ModScreenMaxWidth = 850f;
        private static readonly float ModScreenMinHeight = 50f;
        private static readonly float ModScreenMaxHeight = 550f;

        private static ModAchievements Instance;
        private static HUDManager LocalHUDManager;
        private static MenuInGameManager LocalMenuInGameManager;
        private static AchievementsManager LocalAchievementsManager;
        private static Player LocalPlayer;
        private static CursorManager LocalCursorManager;
        private static MenuDebugAchievements LocalMenuDebugAchievements;

        private static float ModScreenStartPositionX { get; set; } = Screen.width / 2f;
        private static float ModScreenStartPositionY { get; set; } = Screen.height / 2f;
        private static bool IsMinimized { get; set; } = false;
        private Color DefaultGuiColor = GUI.contentColor;
        private bool ShowModUI = false;

        public bool IsModActiveForMultiplayer { get; private set; } = false;
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public static bool AchievementDataLoaded { get; set; } = false;
        public static bool LogAchievementInfoOption { get; private set; } = false;
        public static bool IsLocalMenuDebugAchievementsShown { get; private set; } = false;

        public static Rect LocalDebugMenuAchievementsScreen = new Rect(ModAchievementsScreen.x, ModAchievementsScreen.y, ModAchievementsScreen.width / 2f, ModAchievementsScreen.height / 2f);
        public static Rect ModAchievementsScreen = new Rect(ModScreenStartPositionX, ModScreenStartPositionY, ModScreenTotalWidth, ModScreenTotalHeight);
        public static Vector2 AchievementsScrollViewPosition;
        public static Vector2 LockedAchievementsScrollViewPosition;

        public static AchievementData SelectedAchievementData;
        public static List<string> LocalAchievementsDebugData = new List<string>();
        public static List<AchievementData> LocalAchievementDataList = new List<AchievementData>();

        private static readonly string LogPath = Path.Combine(Application.dataPath.Replace("GH_Data", "Logs"), $"{ModName}.log");
        private static readonly string RuntimeConfiguration = Path.Combine(Application.dataPath.Replace("GH_Data", "Mods"), $"{nameof(RuntimeConfiguration)}.xml");
        private static KeyCode ModKeybindingId { get; set; } = KeyCode.Alpha9;
        private KeyCode GetConfigurableKey(string buttonId)
        {
            KeyCode configuredKeyCode = default;
            string configuredKeybinding = string.Empty;

            try
            {
                if (File.Exists(RuntimeConfiguration))
                {
                    using (var xmlReader = XmlReader.Create(new StreamReader(RuntimeConfiguration)))
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader["ID"] == ModName)
                            {
                                if (xmlReader.ReadToFollowing(nameof(Button)) && xmlReader["ID"] == buttonId)
                                {
                                    configuredKeybinding = xmlReader.ReadElementContentAsString();
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(configuredKeybinding))
                {
                    configuredKeyCode = EnumUtils<KeyCode>.GetValue(configuredKeybinding);
                }
                else
                {
                    if (buttonId == nameof(ModKeybindingId))
                    {
                        configuredKeyCode = ModKeybindingId;
                    }
                }
                return configuredKeyCode;
            }
            catch (Exception exc)
            {
                HandleException(exc, nameof(GetConfigurableKey));
                if (buttonId == nameof(ModKeybindingId))
                {
                    configuredKeyCode = ModKeybindingId;
                }
                return configuredKeyCode;
            }
        }

        public ModAchievements()
        {
            useGUILayout = true;
            ModKeybindingId = KeyCode.Alpha9;
            Instance = this;
        }
        public static ModAchievements Get()
        {
            return Instance;
        }

        public static string HUDBigInfoMessage(string message)
            => $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>System</color>\n{message}";
        public static string OnlyForSinglePlayerOrHostMessage()
                    => $"Only available for single player or when host. Host can activate using ModManager.";
        public static string PermissionChangedMessage(string permission, string reason)
            => $"Permission to use mods and cheats in multiplayer was {permission} because {reason}.";
        public static string HUDBigInfoMessage(string message, MessageType messageType, Color? headcolor = null)
            => $"<color=#{ (headcolor != null ? ColorUtility.ToHtmlStringRGBA(headcolor.Value) : ColorUtility.ToHtmlStringRGBA(Color.red))  }>{messageType}</color>\n{message}";

        private void HandleException(Exception exc, string methodName)
        {
            string info = $"[{ModName}:{methodName}] throws exception:\n{exc.Message}";
            ModAPI.Log.Write(info);
            ShowHUDBigInfo(HUDBigInfoMessage(info, MessageType.Error, Color.red));
        }
        public void ShowHUDBigInfo(string text)
        {
            string header = $"{ModName} Info";
            string textureName = HUDInfoLogTextureType.Count.ToString();
            HUDBigInfo hudBigInfo = (HUDBigInfo)LocalHUDManager.GetHUD(typeof(HUDBigInfo));
            HUDBigInfoData.s_Duration = 2f;
            HUDBigInfoData hudBigInfoData = new HUDBigInfoData
            {
                m_Header = header,
                m_Text = text,
                m_TextureName = textureName,
                m_ShowTime = Time.time
            };
            hudBigInfo.AddInfo(hudBigInfoData);
            hudBigInfo.Show(true);
        }
        public void ShowHUDInfoLog(string itemID, string localizedTextKey)
        {
            var localization = GreenHellGame.Instance.GetLocalization();
            HUDMessages hUDMessages = (HUDMessages)LocalHUDManager.GetHUD(typeof(HUDMessages));
            hUDMessages.AddMessage(
                $"{localization.Get(localizedTextKey)}  {localization.Get(itemID)}"
                );
        }
        private void ToggleShowUI()
        {
            ShowModUI = !ShowModUI;
        }
        private void ModManager_onPermissionValueChanged(bool optionValue)
        {
            string reason = optionValue ? "the game host allowed usage" : "the game host did not allow usage";
            IsModActiveForMultiplayer = optionValue;

            ShowHUDBigInfo(
                          (optionValue ?
                            HUDBigInfoMessage(PermissionChangedMessage($"granted", $"{reason}"), MessageType.Info, Color.green)
                            : HUDBigInfoMessage(PermissionChangedMessage($"revoked", $"{reason}"), MessageType.Info, Color.yellow))
                            );
        }
        private void EnableCursor(bool blockPlayer = false)
        {
            LocalCursorManager.ShowCursor(blockPlayer, false);

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

        public void Start()
        {
            ModManager.ModManager.onPermissionValueChanged += ModManager_onPermissionValueChanged;
            ModKeybindingId = GetConfigurableKey(nameof(ModKeybindingId));
        }

        private void Update()
        {
            if (Input.GetKeyDown(ModKeybindingId))
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
            LocalCursorManager = CursorManager.Get();
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
            if (IsModActiveForSingleplayer || IsModActiveForMultiplayer)
            {
                using (var optionsScope = new GUILayout.VerticalScope(GUI.skin.box))
                {
                    GUI.color = DefaultGuiColor;
                    GUILayout.Label($"Press [Toggle stats] to toggle your achievements screen showing more statistical info: ", GUI.skin.label);
                    if (GUILayout.Button("Toggle stats", GUI.skin.button))
                    {
                        ToggleDebugStatistics();
                    }

                    GUILayout.Label($"Press [Load achievements] to show your current achievement and status colorcoded: ", GUI.skin.label);
                    if (GUILayout.Button("Load achievements", GUI.skin.button))
                    {
                        OnClickLoadAchievementsButton();
                    }
                    if (AchievementDataLoaded)
                    {
                        AchievementsScrollView();
                    }
                }
            }
            else
            {
                OnlyForSingleplayerOrWhenHostBox();
            }
        }

        private void ModOptionsBox()
        {
            if (IsModActiveForSingleplayer || IsModActiveForMultiplayer)
            {
                using (var optionsScope = new GUILayout.VerticalScope(GUI.skin.box))
                {
                    GUILayout.Label($"To toggle the main mod UI, press [{ModKeybindingId}]", GUI.skin.label);
                    LogAchievementInfoOptionBox();
                    MultiplayerOptionBox();
                }
            }
            else
            {
                OnlyForSingleplayerOrWhenHostBox();
            }
        }

        private void OnlyForSingleplayerOrWhenHostBox()
        {
            using (var infoScope = new GUILayout.HorizontalScope(GUI.skin.box))
            {
                GUI.color = Color.yellow;
                GUILayout.Label(OnlyForSinglePlayerOrHostMessage(), GUI.skin.label);
            }
        }

        private void LogAchievementInfoOptionBox()
        {
            try
            {
                using (var logachievementsoptionScope = new GUILayout.VerticalScope(GUI.skin.box))
                {
                    GUI.color = DefaultGuiColor;
                    GUILayout.Label($"Logging option: ", GUI.skin.label);
                    LogAchievementInfoOption = GUILayout.Toggle(LogAchievementInfoOption, $"Log achievement info to {LogPath}?", GUI.skin.toggle);
                }
            }
            catch (Exception exc)
            {
                HandleException(exc, nameof(LogAchievementInfoOptionBox));
            }
        }

        private void MultiplayerOptionBox()
        {
            try
            {
                using (var multiplayeroptionsScope = new GUILayout.VerticalScope(GUI.skin.box))
                {
                    GUILayout.Label("Multiplayer options: ", GUI.skin.label);
                    string multiplayerOptionMessage = string.Empty;
                    if (IsModActiveForSingleplayer || IsModActiveForMultiplayer)
                    {
                        GUI.color = Color.green;
                        if (IsModActiveForSingleplayer)
                        {
                            multiplayerOptionMessage = $"you are the game host";
                        }
                        if (IsModActiveForMultiplayer)
                        {
                            multiplayerOptionMessage = $"the game host allowed usage";
                        }
                        _ = GUILayout.Toggle(true, PermissionChangedMessage($"granted", multiplayerOptionMessage), GUI.skin.toggle);
                    }
                    else
                    {
                        GUI.color = Color.yellow;
                        if (!IsModActiveForSingleplayer)
                        {
                            multiplayerOptionMessage = $"you are not the game host";
                        }
                        if (!IsModActiveForMultiplayer)
                        {
                            multiplayerOptionMessage = $"the game host did not allow usage";
                        }
                        _ = GUILayout.Toggle(false, PermissionChangedMessage($"revoked", $"{multiplayerOptionMessage}"), GUI.skin.toggle);
                    }
                }
            }
            catch (Exception exc)
            {
                HandleException(exc, nameof(MultiplayerOptionBox));
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
                HandleException(exc, nameof(OnClickLoadAchievementsButton));
            }
        }

        private void LoadAchievementsData(bool logInfo = false)
        {
            try
            {
                LocalAchievementDataList.Clear();            
                AchievementInfoLogger.Clear();

                if (LocalAchievementsDebugData == null || LocalAchievementsDebugData.Count == 0)
                {
                    AchievementDataLoaded = false;

                    if (logInfo)
                    {
                        AchievementInfoLogger.AppendLine($"\n{nameof(LocalAchievementsDebugData)} not found!");
                        ModAPI.Log.Write(AchievementInfoLogger.ToString());                      
                    }
                    return;
                }

                if (logInfo)
                {
                    AchievementInfoLogger.AppendLine($" \nApiName\tTitel\tDescription\tIsAchieved\tIconURI");
                }

                foreach (string achievementDebugData in LocalAchievementsDebugData)
                {
                    bool isAchieved = achievementDebugData.Contains("green");
                    string apiName = achievementDebugData.Split('>')[1]?.Split('<')[0];
                    AchievementID id = EnumUtils<AchievementID>.GetValue(apiName);
                    AchievementData achievementData = new AchievementData(apiName);
                    if (achievementData != null)
                    {
                        achievementData.SetAchived(isAchieved);
                        LocalAchievementDataList.Add(achievementData);
                        
                        if (logInfo)
                        {
                            AchievementInfoLogger.AppendLine($"\n{achievementData.GetApiName()}\t{AchievementResource.GetTitle(id)}\t{AchievementResource.GetDescription(id)}\t{achievementData.IsAchieved()}\t{AchievementResource.GetIconFileUriString(id)}");
                        }
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
                AchievementInfoLogger.Clear();
                AchievementInfoLogger.AppendLine($"\n[{ModName}:{nameof(LoadAchievementsData)}({nameof(logInfo)} = {logInfo}] throws exception:\n{exc.Message}");
                ModAPI.Log.Write(AchievementInfoLogger.ToString());
                AchievementDataLoaded = false;
                HandleException(exc, nameof(LoadAchievementsData));
            }
        }

        private void ToggleDebugStatistics()
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

        private void AchievementsScrollView()
        {
            AchievementsScrollViewPosition = GUILayout.BeginScrollView(AchievementsScrollViewPosition, GUI.skin.scrollView, GUILayout.MinHeight(150f));

            if (LocalAchievementDataList != null && LocalAchievementDataList.Count > 0)
            {
                AddAchievementInfoLabels();
            }

            GUILayout.EndScrollView();
        }             

        private void AddAchievementInfoLabels()
        {
            try
            {
                foreach (AchievementData localAchievementData in LocalAchievementDataList)
                {
                    AchievementID id = EnumUtils<AchievementID>.GetValue(localAchievementData.GetApiName());

                    //StartCoroutine(AchievementResource.LoadTexture(delegate (Texture2D icont)
                    //{
                    //    AchievementResource.SteamAchievementIconTexture = icont;
                    //}, AchievementResource.GetIconFileUriString(id)));

                    GUI.contentColor = localAchievementData.IsAchieved() ? Color.green : Color.red;
                    GUIContent content = new GUIContent(AchievementResource.GetTitle(id), AchievementResource.GetDescription(id));

                    using (var horScope = new GUILayout.HorizontalScope(GUI.skin.box))
                    {
                        //GUI.DrawTexture(new Rect(AchievementResource.SteamAchievementIconTexture.width/2f - 40f, 20f, AchievementResource.SteamAchievementIconTexture.width/2f, AchievementResource.SteamAchievementIconTexture.height/2f),
                        //    AchievementResource.SteamAchievementIconTexture);

                        GUILayout.Label(content, GUI.skin.label);

                        if (!localAchievementData.IsAchieved())
                        {
                            if (GUILayout.Button("Unlock", GUI.skin.button, GUILayout.MaxWidth(150f)))
                            {
                                SelectedAchievementData = localAchievementData;
                                OnClickUnlockAchievementButton();
                                OnClickLoadAchievementsButton();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                HandleException(exc, nameof(AddAchievementInfoLabels));
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
                HandleException(exc, nameof(OnClickUnlockAchievementButton));
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
