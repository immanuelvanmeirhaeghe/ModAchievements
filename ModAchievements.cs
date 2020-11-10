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
        private static HUDManager hUDManager;
        private static MenuInGameManager menuInGameManager;

        private static Player player;

        private static readonly string ModName = nameof(ModAchievements);

        private bool ShowUI = false;

        public static Rect ModModAchievementsScreen = new Rect(Screen.width / 40f, Screen.height / 40f, 750f, 250f);

        public static Vector2 ScrollPosition;

        public bool IsModActiveForMultiplayer { get; private set; }
        public bool IsModActiveForSingleplayer => ReplTools.AmIMaster();

        public static string PermissionChangedMessage(string permission) => $"Permission to use mods and cheats in multiplayer was {permission}";

        private static string HUDBigInfoMessage(string message) => $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.cyan)}>System</color>\n{message}";

        public static string OnlyForSinglePlayerOrHostMessage(string message)
           => $"\n<color=#{ColorUtility.ToHtmlStringRGBA(Color.yellow)}>{message}</color> is only available for single player or when host.\nHost can activate using <b>ModManager</b>.";

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
            ModModAchievementsScreen = GUILayout.Window(wid, ModModAchievementsScreen, InitModAchievementsScreen, $"{ModName}", GUI.skin.window);
        }

        private void InitData()
        {
            hUDManager = HUDManager.Get();
            player = Player.Get();
            menuInGameManager = MenuInGameManager.Get();
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
                //GUILayout.Label("Achievements", GUI.skin.label);

                //AchievementsScrollView();

                if (GUILayout.Button("Open manager", GUI.skin.button))
                {
                    OnClickOpenManagerButton();
                    //CloseWindow();
                }
            }
        }

        private void OnClickOpenManagerButton()
        {
            try
            {
                var manager = (MenuDebugAchievements) menuInGameManager.GetMenu(typeof(MenuDebugAchievements));
                if (manager != null)
                {
                    manager.Show();
                }

            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{ModName}:{nameof(OnClickOpenManagerButton)}] throws exception:\n{exc.Message}");
            }
        }

        private void AchievementsScrollView()
        {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUI.skin.scrollView, GUILayout.MinHeight(300f));

            //SelectedItemIndex = GUILayout.SelectionGrid(SelectedItemIndex, GetItems(), 3, GUI.skin.button);

            GUILayout.EndScrollView();
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

        public void ShowHUDBigInfo(string text)
        {
            string header = $"{ModName} Info";
            string textureName = HUDInfoLogTextureType.Count.ToString();

            HUDBigInfo bigInfo = (HUDBigInfo)hUDManager.GetHUD(typeof(HUDBigInfo));
            HUDBigInfoData.s_Duration = 5f;
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

        public void ShowHUDInfoLog(string itemID, string localizedTextKey)
        {
            var localization = GreenHellGame.Instance.GetLocalization();
            HUDMessages hUDMessages = (HUDMessages)hUDManager.GetHUD(typeof(HUDMessages));
            hUDMessages.AddMessage(
                $"{localization.Get(localizedTextKey)}  {localization.Get(itemID)}"
                );
        }

        private void EnableCursor(bool blockPlayer = false)
        {
            CursorManager.Get().ShowCursor(blockPlayer, false);

            if (blockPlayer)
            {
                player.BlockMoves();
                player.BlockRotation();
                player.BlockInspection();
            }
            else
            {
                player.UnblockMoves();
                player.UnblockRotation();
                player.UnblockInspection();
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
