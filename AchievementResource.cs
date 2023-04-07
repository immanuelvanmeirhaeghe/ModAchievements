using ModAchievements.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static Mono.Security.X509.X520;

namespace ModAchievements
{
    public static class AchievementResource
    {
        public static readonly string SteamAchievementIconBaseUri = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/815370/";
        public static readonly string SteamAchievementsUri = $"https://steamcommunity.com/stats/815370/achievements/";

        public static string SteamAchievementIconFileUriString;
        public static Texture2D SteamAchievementIconTexture;

        public static string GetIconFileUriString(AchievementID id)
        {
            string iconFileUriString;
            switch (id)
            {
                case AchievementID.ACH_TUTORIAL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}9eee0f2737e3505f42e990aa37b946ba6a505ace.jpg";
                    break;
                case AchievementID.ACH_AYAHUASKA:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}7d48ff16f071bab1810bf23d10824e949f725f8b.jpg";
                    break;
                case AchievementID.ACH_GOOD_ENDING:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}ddac862e7273d838e86975a3fb96e40bb67c0b16.jpg";
                    break;
                case AchievementID.ACH_BAD_ENDING:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}b03649f43c09f0f7c29af7a17a9d4c42bb14e7cd.jpg";
                    break;
                case AchievementID.ACH_GREEDY:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}adc3eceb6ba2589b75b30b917d1fa193ef6f59f0.jpg";
                    break;
                case AchievementID.ACH_ENDING_GREENHELL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}0c149b315e808c7294b9ef34dd2500def5d5c28a.jpg";
                    break;
                case AchievementID.ACH_TRAVEL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}3d3d02fc0e5b463ed75d2e6e433da616fda7834b.jpg";
                    break;
                case AchievementID.ACH_START_FIRE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}07d95d570f03ba93dd94365147a55c22906cb328.jpg";
                    break;
                case AchievementID.ACH_WELCOME:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}cec0569cafe5e282f13f55f25f062b49c1f5f41b.jpg";
                    break;
                case AchievementID.ACH_SURVIVE_10:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}f25676215bfaaf27b06b39f388fad16d1b396d02.jpg";
                    break;
                case AchievementID.ACH_JUST_DIE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}538d9164de02f1f1becc25727a99c4d9a3c33fbf.jpg";
                    break;
                case AchievementID.ACH_CANIBAL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}1d6725a911ecb2236852a80ee32b1fd6dc08744b.jpg";
                    break;
                case AchievementID.ACH_KILL_TRIBE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}4bb2632dac0ce10bd6ede691a694f8d26ed0d03f.jpg";
                    break;
                case AchievementID.ACH_1ST_TOOL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}b721c3ad4c5bfa8daccc110510ac4afa8d134b53.jpg";
                    break;
                case AchievementID.ACH_VEGAN:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}54f083013b95dbba22955c590ecc9a85ab12dd03.jpg";
                    break;
                case AchievementID.ACH_PACIFIST:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}1acca53a93784d3411079184ef8f37c585926c60.jpg";
                    break;
                case AchievementID.ACH_WILLSON:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}24ea232dff9cf50e2f9520a19aabf5c90d0e2361.jpg";
                    break;
                case AchievementID.ACH_MAX_SKILL:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}54f083013b95dbba22955c590ecc9a85ab12dd03.jpg";
                    break;
                case AchievementID.ACH_IRONMAN:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}1959e8c3fe87ac893f773da2132e3361cc3a94e8.jpg";
                    break;
                case AchievementID.ACH_SAVEGAME:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}e202c4dc5e42b075aba166cd06627b94e48d918a.jpg";
                    break;
                case AchievementID.ACH_FIND_STORYCAVE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}882c15a5c6ed442cd9d1ae7151c8ada83ddf54f4.jpg";
                    break;
                case AchievementID.ACH_SANITY_TRIBE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}e08a8f3b2b4c211f47301a9723572f27e9698041.jpg";
                    break;
                case AchievementID.ACH_LEECHES:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}82250f4a905f95016a8fc59e90803a7e73a6129f.jpg";
                    break;
                case AchievementID.ACH_HOME:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}af12a4562fe45bcf9bdce5204c6e301266fc8ee1.jpg";
                    break;
                case AchievementID.ACH_CURE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}f9bfc0c3853eadd43850ffcc0e47e26954ea6b14.jpg";
                    break;
                case AchievementID.ACH_SAFE_WATER:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}9b648cf8b09e1c167a0c5d02be2b9e92d92ea966.jpg";
                    break;
                case AchievementID.ACH_INSOMIA:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}bb3f32820073357b16cb3a6abd56b72fbdff6ed3.jpg";
                    break;
                case AchievementID.ACH_FIREPLACE_GOING:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}91e032b74cec0b764977d7043ea7086f983ba73e.jpg";
                    break;
                case AchievementID.ACH_TURTLE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}2e6452b2725d1d15dbec5cf0871ae16d3aa966a2.jpg";
                    break;
                case AchievementID.ACH_FISHING:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}3fb836d98fc2ae1e1309ce3d541a615db3cf7a6d.jpg";
                    break;
                case AchievementID.ACH_KING_OF_THE_JUNGLE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}fb98947cabdcd410db35fc38870b350c247ffef1.jpg";
                    break;
                case AchievementID.ACH_SNOWMAN:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}479b799bf67e8bf7b16310f3b36fe2304750f36a.jpg";
                    break;
                case AchievementID.ACH_ALL_FIRE_TOOLS:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}04cd5412c0a324a7f76603d387eb11174ffccc6f.jpg";
                    break;
                case AchievementID.ACH_CATCH_AT_ONCE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}92903825193ca8578b3b2a44afe93e28b17b7c83.jpg";
                    break;
                case AchievementID.ACH_FARMER:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}c69dd38e2faa031bbba4c86edb3002e975e74aa5.jpg";
                    break;
                case AchievementID.ACH_ALL_READABLE:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}49121d04b806a5cdbf77773c9ea5c6e7a54c7d7f.jpg";
                    break;
                case AchievementID.ACH_MAGGOT_WOUND:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}deb806485c0a3dd2b50b11b02c336acff9430107.jpg";
                    break;
                case AchievementID.ACH_ALL_SICKNESS:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}bd3fd7d4fa4d6b9f7f9fa3ede94d3580d5fc652a.jpg";
                    break;
                case AchievementID.ACH_ALL_CHALLENGES:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}aff8c61825b9f758e3351912972ad6709d252387.jpg";
                    break;
                case AchievementID.ACH_FULL_MAP:
                    iconFileUriString = $"{SteamAchievementIconBaseUri}709cf04c4bc39012d62e9bc37b03dad0b34dc289.jpg";
                    break;                    
                default:
                    iconFileUriString = string.Empty;
                    break;
            }
            SteamAchievementIconFileUriString = iconFileUriString;
            return iconFileUriString;
        }

        public static IEnumerator GetIconTexture(string uriString)
        {
            using (var webRequest = UnityWebRequestTexture.GetTexture(uriString))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    ModAPI.Log.Write($"[{nameof(AchievementResource)}:{nameof(GetIconTexture)}({nameof(uriString)}={uriString})]  web request encountered an error:\n{webRequest.error}");
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(webRequest);
                    SteamAchievementIconTexture = texture;
                }
            }
        }

        public static string GetTitle(AchievementID id)
        {
            string Title;
            switch (id)
            {
                case AchievementID.ACH_TUTORIAL:
                    Title = $"You are not prepared";
                    break;
                case AchievementID.ACH_AYAHUASKA:
                    Title = $"Soul Vine";
                    break;
                case AchievementID.ACH_GOOD_ENDING:
                    Title = $"Just, wait for me…";
                    break;
                case AchievementID.ACH_BAD_ENDING:
                    Title = $"It's all over, again";
                    break;
                case AchievementID.ACH_GREEDY:
                    Title = $"Greedy";
                    break;
                case AchievementID.ACH_ENDING_GREENHELL:
                    Title = $"Green Hell";
                    break;
                case AchievementID.ACH_TRAVEL:
                    Title = $"Globetrotter";
                    break;
                case AchievementID.ACH_START_FIRE:
                    Title = $"I made fire!";
                    break;
                case AchievementID.ACH_WELCOME:
                    Title = $"Welcome to the jungle";
                    break;
                case AchievementID.ACH_SURVIVE_10:
                    Title = $"I made it";
                    break;
                case AchievementID.ACH_JUST_DIE:
                    Title = $"The first step to greatness";
                    break;
                case AchievementID.ACH_CANIBAL:
                    Title = $"Tastes like chicken...";
                    break;
                case AchievementID.ACH_KILL_TRIBE:
                    Title = $"Self-defense";
                    break;
                case AchievementID.ACH_1ST_TOOL:
                    Title = $"Caveman";
                    break;
                case AchievementID.ACH_VEGAN:
                    Title = $"Vegan!";
                    break;
                case AchievementID.ACH_PACIFIST:
                    Title = $"Pacifist";
                    break;
                case AchievementID.ACH_WILLSON:
                    Title = $"Casted Far Away";
                    break;
                case AchievementID.ACH_MAX_SKILL:
                    Title = $"Making progress";
                    break;
                case AchievementID.ACH_IRONMAN:
                    Title = $"Iron Man";
                    break;
                case AchievementID.ACH_SAVEGAME:
                    Title = $"I'm saved";
                    break;
                case AchievementID.ACH_FIND_STORYCAVE:
                    Title = $"It's all their fault";
                    break;
                case AchievementID.ACH_SANITY_TRIBE:
                    Title = $"Am I losing it?";
                    break;
                case AchievementID.ACH_LEECHES:
                    Title = $"Leeches, leeches everywhere";
                    break;
                case AchievementID.ACH_HOME:
                    Title = $"Home Sweet Home";
                    break;
                case AchievementID.ACH_CURE:
                    Title = $"I have it!";
                    break;
                case AchievementID.ACH_SAFE_WATER:
                    Title = $"Fresh Water";
                    break;
                case AchievementID.ACH_INSOMIA:
                    Title = $"I don't need to sleep";
                    break;
                case AchievementID.ACH_FIREPLACE_GOING:
                    Title = $"Keeper of the flame";
                    break;
                case AchievementID.ACH_TURTLE:
                    Title = $"Going back home";
                    break;
                case AchievementID.ACH_FISHING:
                    Title = $"Gotcha!";
                    break;
                case AchievementID.ACH_KING_OF_THE_JUNGLE:
                    Title = $"King of the jungle";
                    break;
                case AchievementID.ACH_SNOWMAN:
                    Title = $"Do you want to play with a snowman?";
                    break;
                case AchievementID.ACH_ALL_FIRE_TOOLS:
                    Title = $"Pyromaniac";
                    break;
                case AchievementID.ACH_CATCH_AT_ONCE:
                    Title = $"Got to catch them all";
                    break;
                case AchievementID.ACH_FARMER:
                    Title = $"Gardener";
                    break;
                case AchievementID.ACH_ALL_READABLE:
                    Title = $"Librarian";
                    break;
                case AchievementID.ACH_MAGGOT_WOUND:
                    Title = $"Improvise, adapt, survive";
                    break;
                case AchievementID.ACH_ALL_SICKNESS:
                    Title = $"Mr... I don't feel so good";
                    break;
                case AchievementID.ACH_ALL_CHALLENGES:
                    Title = $"I'm not afraid of any work";
                    break;
                case AchievementID.ACH_FULL_MAP:
                    Title = $"Cartographer";
                    break;

                case AchievementID.ACH_NEW_FRIEND:
                    Title = $"A new friend";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_PLANTING:
                    Title = $"Mu'agi Friend";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_HUNTING:
                    Title = $"Un'garaca Friend";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_FISHING:
                    Title = $"Habbacu Friend";
                    break;

                case AchievementID.ACH_BRING_9_KIDS_BACK:
                    Title = $"Babysitter";
                    break;

                case AchievementID.ACH_HEAL_10_TRIBES:
                    Title = $"Molineria-man";
                    break;

                case AchievementID.ACH_BURN_10_CORPSES:
                    Title = $"Rest in peace";
                    break;

                case AchievementID.ACH_BURN_10_TRASHES:
                    Title = $"Cleaning volunteer";
                    break;

                case AchievementID.ACH_HELP_5_FISHERMEN:
                    Title = $"Fishing in troubled waters";
                    break;

                case AchievementID.ACH_REBUILD_10_TOTEMS:
                    Title = $"Handyman";
                    break;

                case AchievementID.ACH_FREE_9_WOMEN:
                    Title = $"Cage opener";
                    break;

                case AchievementID.ACH_FIND_5_MAPS:
                    Title = $"Map Collector";
                    break;

                case AchievementID.ACH_COMPLETE_SOA3_STORY:
                    Title = $"This is how it began";
                    break;

                case AchievementID.RAPORT_BACK_TO_THE_BASE:
                    Title = $"This is Jake Higgins...";
                    break;

                case AchievementID.COMPLETE_MUAGI_TRIAL:
                    Title = $"The Ritual of Mu'agi";
                    break;

                case AchievementID.COMPLETE_HABBAKU_TRIAL:
                    Title = $"The Ritual of Habbacu";
                    break;

                case AchievementID.COMPLETE_UNGARACA_TRIAL:
                    Title = $"The Ritual of Un'garaca";
                    break;

                case AchievementID.REACH_YABAHUACA_VILLAGE:
                    Title = $"Are we there yet?";
                    break;

                case AchievementID.PASS_YABAHUACA_TRIAL:
                    Title = $"Work hard, play hard";
                    break;

                case AchievementID.DISCOVER_VILLAGE_MUAGI:
                    Title = $"Oyohua Mu'agi";
                    break;

                case AchievementID.DISCOVER_VILLAGE_HABBAKU:
                    Title = $"Oyohua Habbacu";
                    break;

                case AchievementID.DISCOVER_VILLAGE_UNGARACA:
                    Title = $"Oyohua Un'garaca";
                    break;

                case AchievementID.COMPLETE_ALL_MUAGI_LEGENDS:
                    Title = $"The Legends of Mu'agi";
                    break;

                case AchievementID.COMPLETE_ALL_UNGARACA_LEGENDS:
                    Title = $"The Legends of Un'garaca";
                    break;

                case AchievementID.COMPLETE_ALL_HABBAKU_LEGENDS:
                    Title = $"The Legends of Habbacu";
                    break;

                case AchievementID.COMPLETE_ALL_SOA_ACHIEVEMENTS:
                    Title = $"Thats the spirit!";
                    break;

                case AchievementID.ACH_EMOTIONAL_SUPPORT:
                    Title = $"Emotional support";
                    break;

                case AchievementID.ACH_CIRCLE_OF_LIFE:
                    Title = $"Circle of Life";
                    break;

                default:
                    Title = string.Empty;
                    break;
            }
            return Title;
        }

        public static string GetDescription(AchievementID id)
        {
            string Description;
            switch (id)
            {
                case AchievementID.ACH_TUTORIAL:
                    Description = $"Finish the tutorial";
                    break;
                case AchievementID.ACH_AYAHUASKA:
                    Description = $"Drink Ayahuasca";
                    break;
                case AchievementID.ACH_GOOD_ENDING:
                    Description = $"Finish the Story - \"Good\" Ending";
                    break;
                case AchievementID.ACH_BAD_ENDING:
                    Description = $"Finish the Story - \"Bad\" Ending";
                    break;
                case AchievementID.ACH_GREEDY:
                    Description = $"Complete Story mode on any difficulty with gold sack in backpack";
                    break;
                case AchievementID.ACH_ENDING_GREENHELL:
                    Description = $"Finish the game on Green Hell difficulty level";
                    break;
                case AchievementID.ACH_TRAVEL:
                    Description = $"Travel 64 km";
                    break;
                case AchievementID.ACH_START_FIRE:
                    Description = $"Start a fire";
                    break;
                case AchievementID.ACH_WELCOME:
                    Description = $"Survive 1 night in the jungle";
                    break;
                case AchievementID.ACH_SURVIVE_10:
                    Description = $"Survive 10 days on King of the Jungle difficulty or higher";
                    break;
                case AchievementID.ACH_JUST_DIE:
                    Description = $"Die";
                    break;
                case AchievementID.ACH_CANIBAL:
                    Description = $"Eat human meat";
                    break;
                case AchievementID.ACH_KILL_TRIBE:
                    Description = $"Kill a tribesman";
                    break;
                case AchievementID.ACH_1ST_TOOL:
                    Description = $"Craft your first tool";
                    break;
                case AchievementID.ACH_VEGAN:
                    Description = $"Survive 25 days solely on mushroom and plant-based food on Welcome to the Jungle difficulty or higher";
                    break;
                case AchievementID.ACH_PACIFIST:
                    Description = $"Survive 10 days on King of the Jungle difficulty or higher without killing animals, humans, destroying bee nests and interacting with traps";
                    break;
                case AchievementID.ACH_WILLSON:
                    Description = $"Casted Far Away";
                    break;
                case AchievementID.ACH_MAX_SKILL:
                    Description = $"Reach Max at any skill";
                    break;
                case AchievementID.ACH_IRONMAN:
                    Description = $"Create and wear a full metal armor set";
                    break;
                case AchievementID.ACH_SAVEGAME:
                    Description = $"Save your game in a shelter";
                    break;
                case AchievementID.ACH_FIND_STORYCAVE:
                    Description = $"It's all their fault";
                    break;
                case AchievementID.ACH_SANITY_TRIBE:
                    Description = $"Kill a Sanity tribesman";
                    break;
                case AchievementID.ACH_LEECHES:
                    Description = $"Remove 50 leeches from your body";
                    break;
                case AchievementID.ACH_HOME:
                    Description = $"Build your first shelter";
                    break;
                case AchievementID.ACH_CURE:
                    Description = $"Find the Cure";
                    break;
                case AchievementID.ACH_SAFE_WATER:
                    Description = $"Drink safe water";
                    break;
                case AchievementID.ACH_INSOMIA:
                    Description = $"Get 5 stacks of insomnia";
                    break;
                case AchievementID.ACH_FIREPLACE_GOING:
                    Description = $"Keep a single fire burning for over 5 days on Welcome to the jungle difficulty or higher";
                    break;
                case AchievementID.ACH_TURTLE:
                    Description = $"Make tortoise soup in it's shell";
                    break;
                case AchievementID.ACH_FISHING:
                    Description = $"Catch 9 aquatic animals";
                    break;
                case AchievementID.ACH_KING_OF_THE_JUNGLE:
                    Description = $"Hunt a Rattlesnake, Jaguar, Puma, Caiman and 3 types of arachnids";
                    break;
                case AchievementID.ACH_SNOWMAN:
                    Description = $"Find a snowman package in 3 different locations";
                    break;
                case AchievementID.ACH_ALL_FIRE_TOOLS:
                    Description = $"Unlock 4 fire starting tools";
                    break;
                case AchievementID.ACH_CATCH_AT_ONCE:
                    Description = $"Experience 12 unique diseases and wounds";
                    break;
                case AchievementID.ACH_FARMER:
                    Description = $"Cultivate 12 different plants";
                    break;
                case AchievementID.ACH_ALL_READABLE:
                    Description = $"Read 50 collectibles";
                    break;
                case AchievementID.ACH_MAGGOT_WOUND:
                    Description = $"Let maggots eat your infected wound";
                    break;
                case AchievementID.ACH_ALL_SICKNESS:
                    Description = $"At the same time get leeches, worm, rash, fever, poison, food poisoning, parasites, insomnia, dirt and any wound";
                    break;
                case AchievementID.ACH_ALL_CHALLENGES:
                    Description = $"Complete 7 Challenges";
                    break;
                case AchievementID.ACH_FULL_MAP:
                    Description = $"Unlock 60 places on the map";
                    break;

                case AchievementID.ACH_NEW_FRIEND:
                    Description = $"Put an animal into Animal Pen";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_PLANTING:
                    Description = $"Gain 800 trust of the Mu'agi tribe";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_HUNTING:
                    Description = $"Gain 800 trust of the Un'garaca tribe";
                    break;

                case AchievementID.ACH_GAIN_REPUTATION_800_FISHING:
                    Description = $"Gain 800 trust of the Habbacu tribe";
                    break;

                case AchievementID.ACH_BRING_9_KIDS_BACK:
                    Description = $"Return 9 kids to safety";
                    break;

                case AchievementID.ACH_HEAL_10_TRIBES:
                    Description = $" a Tribe Member";
                    break;

                case AchievementID.ACH_BURN_10_CORPSES:
                    Description = $"Burn 10 Tribe Members' corpses";
                    break;

                case AchievementID.ACH_BURN_10_TRASHES:
                    Description = $"Burn 10 Toxic Waste Piles";
                    break;

                case AchievementID.ACH_HELP_5_FISHERMEN:
                    Description = $"Help 5 Tribe Fishermen";
                    break;

                case AchievementID.ACH_REBUILD_10_TOTEMS:
                    Description = $"Rebuild 10 Tribe Totems";
                    break;

                case AchievementID.ACH_FREE_9_WOMEN:
                    Description = $"Free 9 Tribe Women";
                    break;

                case AchievementID.ACH_FIND_5_MAPS:
                    Description = $"Find 5 maps";
                    break;

                case AchievementID.ACH_COMPLETE_SOA3_STORY:
                    Description = $"Complete the Spirits of Amazonia story";
                    break;

                case AchievementID.RAPORT_BACK_TO_THE_BASE:
                    Description = $"Report back to base";
                    break;

                case AchievementID.COMPLETE_MUAGI_TRIAL:
                    Description = $"Complete the Mu'agi Trial";
                    break;

                case AchievementID.COMPLETE_HABBAKU_TRIAL:
                    Description = $"Complete the Habbacu Trial";
                    break;

                case AchievementID.COMPLETE_UNGARACA_TRIAL:
                    Description = $"Complete the Un'garaca Trial";
                    break;

                case AchievementID.REACH_YABAHUACA_VILLAGE:
                    Description = $"Reach the Yabahuaca village";
                    break;

                case AchievementID.PASS_YABAHUACA_TRIAL:
                    Description = $"Complete the Yabahuaca Trial";
                    break;

                case AchievementID.DISCOVER_VILLAGE_MUAGI:
                    Description = $"Discover the Mu'agi Village";
                    break;

                case AchievementID.DISCOVER_VILLAGE_HABBAKU:
                    Description = $"Discover the Habbacu Village";
                    break;

                case AchievementID.DISCOVER_VILLAGE_UNGARACA:
                    Description = $"Discover the Un'garaca Village";
                    break;

                case AchievementID.COMPLETE_ALL_MUAGI_LEGENDS:
                    Description = $"Complete all Mu'agi Legends";
                    break;

                case AchievementID.COMPLETE_ALL_UNGARACA_LEGENDS:
                    Description = $"Complete all Un'garaca Legends";
                    break;

                case AchievementID.COMPLETE_ALL_HABBAKU_LEGENDS:
                    Description = $"Complete all Habbacu Legends";
                    break;

                case AchievementID.COMPLETE_ALL_SOA_ACHIEVEMENTS:
                    Description = $"Complete all Spirits of Amazonia achievements";
                    break;

                case AchievementID.ACH_EMOTIONAL_SUPPORT:
                    Description = $"Pet an animal";
                    break;

                case AchievementID.ACH_CIRCLE_OF_LIFE:
                    Description = $"Breed an animal";
                    break;

                default:
                    Description = string.Empty;
                    break;
            }
            return Description;
        }
    }
}
