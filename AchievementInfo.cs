using Enums;
using ModAchievements.Enums;
using System;
using System.Collections;
using UnityEngine;
using Event = Enums.Event;

namespace ModAchievements
{
    public class AchievementInfo : MonoBehaviour
    {
        public AchievementID AchievementID { get; set; }

        public AchievementData AchievementData { get; set; }

        public AchivementsEvent AchievementEvent { get; set; }

        public Uri AchievementIconFileUri { get; set; }

        public string AchievementIconFileName { get; set; }

        public Texture2D AchievementIconTexture { get; set; }

        public string AchievementTitle { get; set; }

        public string AchievementDescription { get; set; }

        public IEnumerator StartGetTexture(string iconFileUriString)
        {
            yield return AchievementResource.GetIconTexture(iconFileUriString);
        }

        public AchievementInfo(string apiName = "ACH_UNKNOWN", AchievementData achievementData = default)
        {
            try
            {
                AchievementID = EnumUtils<AchievementID>.GetValue(apiName.ToUpper());
                AchievementData = achievementData;
                AchievementEvent = GetEvent(AchievementID);
                AchievementIconFileUri = GetIconFileUri(AchievementID);

                AchievementIconFileName = GetIconFileName(AchievementID);
                AchievementTitle = GetTitle(AchievementID);
                AchievementDescription = GetDescription(AchievementID);
            }
            catch (Exception exc)
            {
                ModAPI.Log.Write($"[{nameof(AchievementInfo)}({nameof(apiName)} = {apiName}, {nameof(achievementData)} = {achievementData}] throws exception:\n{exc.Message}");
                throw exc;
            }
        }

        private string GetIconFileName(AchievementID achievementID)
        {
            string iconFileName = string.Empty;
            string iconFileUriString = AchievementResource.GetIconFileUriString(achievementID);
           
            if (!string.IsNullOrEmpty(iconFileUriString))
            {
                iconFileName = iconFileUriString.Replace(AchievementResource.SteamAchievementIconBaseUri, string.Empty);
            }
            return iconFileName;
        }

        private AchivementsEvent GetEvent(AchievementID achievementID)
        {
            Event event_type;
            switch (achievementID)
            {
                case AchievementID.ACH_UNKNOWN:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_TUTORIAL:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_AYAHUASKA:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_GOOD_ENDING:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_BAD_ENDING:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_GREEDY:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_ENDING_GREENHELL:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_TRAVEL:
                    event_type = Event.TraveledDist;
                    break;
                case AchievementID.ACH_START_FIRE:
                    event_type = Event.IgniteFire;
                    break;
                case AchievementID.ACH_WELCOME:
                    event_type = Event.DaysSurvived;
                    break;
                case AchievementID.ACH_SURVIVE_10:
                    event_type = Event.DaysSurvived;
                    break;
                case AchievementID.ACH_JUST_DIE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_CANIBAL:
                    event_type = Event.Eat;
                    break;
                case AchievementID.ACH_KILL_TRIBE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_1ST_TOOL:
                    event_type = Event.Craft;
                    break;
                case AchievementID.ACH_VEGAN:
                    event_type = Event.Eat;
                    break;
                case AchievementID.ACH_PACIFIST:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_WILLSON:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_MAX_SKILL:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_IRONMAN:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_SAVEGAME:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_FIND_STORYCAVE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_SANITY_TRIBE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_LEECHES:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_HOME:
                    event_type = Event.Build;
                    break;
                case AchievementID.ACH_CURE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_SAFE_WATER:
                    event_type = Event.Drink;
                    break;
                case AchievementID.ACH_INSOMIA:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_FIREPLACE_GOING:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_TURTLE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_FISHING:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_KING_OF_THE_JUNGLE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_SNOWMAN:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_ALL_FIRE_TOOLS:
                    event_type = Event.Craft;
                    break;
                case AchievementID.ACH_CATCH_AT_ONCE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_FARMER:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_ALL_READABLE:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_MAGGOT_WOUND:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_ALL_SICKNESS:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_ALL_CHALLENGES:
                    event_type = Event.None;
                    break;
                case AchievementID.ACH_FULL_MAP:
                    event_type = Event.None;
                    break;
                default:
                    event_type = Event.None;
                    break;
            }

            switch (event_type)
            {
                case Event.DaysSurvived:
                    return AchivementsEvent.DaysSurvived;
                case Event.Eat:
                    return AchivementsEvent.Eat;
                case Event.Drink:
                    return AchivementsEvent.Drink;
                case Event.Sleep:
                    return AchivementsEvent.Sleep;
                case Event.Build:
                    return AchivementsEvent.Build;
                case Event.Craft:
                    return AchivementsEvent.Craft;
                case Event.HealWound:
                    return AchivementsEvent.HealWound;
                case Event.TakeItem:
                    return AchivementsEvent.TakeItem;
                case Event.IgniteFire:
                    return AchivementsEvent.IgniteFire;
                case Event.TraveledDist:
                    return AchivementsEvent.TraveledDist;
                default:
                    return AchivementsEvent.None;
            }
        }

        private Uri GetIconFileUri(AchievementID achievementID)
        {
            Uri iconFileUri = default;
           string iconFileUriString = AchievementResource.GetIconFileUriString(achievementID);

            if (!string.IsNullOrEmpty(iconFileUriString))
            {
                iconFileUri = new Uri(iconFileUriString);
            }
            return iconFileUri;
        }

        //private IEnumerator GetIconTexture(string iconFileUriString)
        //{
        //    yield return AchievementResource.GetIconTexture(iconFileUriString);
        //}

        private string GetTitle(AchievementID achievementID)
        {
            string title = AchievementResource.GetTitle(achievementID);
            return title;
        }

        private string GetDescription(AchievementID achievementID)
        {
            string descr = AchievementResource.GetDescription(achievementID);
            return descr;
        }
    }
}
