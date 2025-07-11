﻿using System;
using System.Collections.Generic;
using MacroTools.DialogueSystem;
using MacroTools.FactionSystem;
using MacroTools.ObjectiveSystem;
using MacroTools.ObjectiveSystem.Objectives.LegendBased;
using MacroTools.Systems;
using WarcraftLegacies.Shared.FactionObjectLimits;
using WarcraftLegacies.Source.Quests;
using WarcraftLegacies.Source.Quests.Warsong;
using WarcraftLegacies.Source.Setup;
using WCSharp.Shared.Data;
using MacroTools.Extensions;
using MacroTools.FactionChoices;
using MacroTools.ResearchSystems;
using WarcraftLegacies.Source.FactionMechanics.Warsong;
using WarcraftLegacies.Source.Researches;


namespace WarcraftLegacies.Source.Factions
{
    public sealed class Warsong : Faction
    {
        private readonly PreplacedUnitSystem _preplacedUnitSystem;
        private readonly AllLegendSetup _allLegendSetup;
        private readonly ArtifactSetup _artifactSetup;

        /// <inheritdoc />

        public Warsong(PreplacedUnitSystem preplacedUnitSystem, AllLegendSetup allLegendSetup, ArtifactSetup artifactSetup)
          : base("Warsong", new[] {PLAYER_COLOR_ORANGE, PLAYER_COLOR_YELLOW, PLAYER_COLOR_COAL},
          @"ReplaceableTextures\CommandButtons\BTNHellScream.blp")
        {
            TraditionalTeam = TeamSetup.Kalimdor;
            _preplacedUnitSystem = preplacedUnitSystem;
            _allLegendSetup = allLegendSetup;
            _artifactSetup = artifactSetup;
            UndefeatedResearch = FourCC("R05W");
            StartingGold = 200;
            CinematicMusic = "DarkAgents";
            ControlPointDefenderUnitTypeId = UNIT_N0D6_CONTROL_POINT_DEFENDER_WARSONG;
            IntroText = $"You are playing as the fierce and relentless {PrefixCol}Warsong Clan|r.\n\n" +
                        "Begin swiftly by rescuing your Chieftain, Grom Hellscream, who is trapped in battle and consumed by demonic fury. His survival is paramount.\n\n" +
                        "With Grom secured, expand your dominance by subduing or pillaging nearby races to bolster your clan's strength.\n\n" +
                        "Work closely with your new elven allies—only together can you overcome the looming threat of the Old Gods.";

            GoldMines = new List<unit>
      {
        _preplacedUnitSystem.GetUnit(FourCC("ngol"), new Point(-9729, 2426)),
      };
            Nicknames = new List<string>
      {
        "ws",
        "war"
      };
            ProcessObjectInfo(WarsongObjectInfo.GetAllObjectLimits());
        }

        /// <inheritdoc />
        public override void OnRegistered()
        {
            RegisterObjectLevels();
            ReplaceWithFactionUnits(this);
            RegisterQuests();
            RegisterDialogue();
            RegisterFlightPath();
            BloodPactBattleSimulation.StartSimulation();
            SharedFactionConfigSetup.AddSharedFactionConfig(this);
            Regions.BarrenAmbient2.CleanupHostileUnits();
            Regions.AshenvaleCreeps.CleanupHostileUnits();
            var thunderBluffUnit = _preplacedUnitSystem.GetUnit(Constants.UNIT_N03M_THUNDERBLUFF);
            thunderBluffUnit.SetOwner(Player(PLAYER_NEUTRAL_AGGRESSIVE));
            var echoIslesUnit = _preplacedUnitSystem.GetUnit(Constants.UNIT_N02V_ECHO_ISLES);
            echoIslesUnit.SetOwner(Player(PLAYER_NEUTRAL_AGGRESSIVE));
        }

        private void RegisterObjectLevels()
        {
          ModAbilityAvailability(ABILITY_ABTL_BATTLE_STATIONS_FROSTWOLF_WARSONG_BURROW, 1);
          ModAbilityAvailability(ABILITY_A0GM_FOR_THE_HORDE_PINK_GREY_MAIN_BUILDINGS, 1);
        }

        private void RegisterQuests()
        {
          StartingQuest = AddQuest(new QuestGrom(_preplacedUnitSystem, _allLegendSetup.Warsong.GromHellscream, _allLegendSetup.Warsong.Gargok));
            AddQuest(new QuestOrgrimmar(Regions.Orgrimmar, _allLegendSetup.Warsong.GromHellscream));
            AddQuest(new QuestCrossroads(Regions.Crossroads));
            AddQuest(new QuestRokhan(_preplacedUnitSystem.GetUnit(Constants.UNIT_MD25_DARKSPEAR_CHAMPION_WARSONG)));
           // AddQuest(new QuestFountainOfBlood(_allLegendSetup.Neutral.FountainOfBlood, _allLegendSetup.Warsong.GromHellscream));
           // AddQuest(new QuestBloodpact(_allLegendSetup.Warsong.Mannoroth, _allLegendSetup.Warsong.GromHellscream));
            AddQuest(new QuestGarrosh(_allLegendSetup.BlackEmpire.Nzoth));
            AddQuest(new QuestWarsongKillCthun(_allLegendSetup.Ahnqiraj.Cthun));
            AddQuest(new QuestKillOldGods(_allLegendSetup.Ahnqiraj.Cthun, _allLegendSetup.BlackEmpire.Nzoth));
            AddQuest(new QuestWarsongHold());
            AddQuest(new QuestExtractSunwellVial(_allLegendSetup.Quelthalas.Sunwell, _artifactSetup.SunwellVial));
            AddQuest(new QuestSubdueOgres(Regions.StonemaulKeep, _allLegendSetup.Warsong, _allLegendSetup.Warsong.GromHellscream));
            AddQuest(new QuestSubdueTrolls(Regions.EchoUnlock, _allLegendSetup.Warsong, _allLegendSetup.Warsong.GromHellscream));
            AddQuest(new QuestSubdueTauren(Regions.ThunderBluff,_allLegendSetup.Warsong, _allLegendSetup.Warsong.GromHellscream, _artifactSetup));
            

        }
        
        private void ReplaceWithFactionUnits(Faction pickedFaction)
        {
          if (pickedFaction == null)
            throw new ArgumentNullException(nameof(pickedFaction), "pickedFaction cannot be null.");
          
          FactionChoiceDialogPresenter.ReplaceRegionUnitsWithFactionEquivalents(Regions.ThunderBluff, pickedFaction);
          FactionChoiceDialogPresenter.ReplaceRegionUnitsWithFactionEquivalents(Regions.EchoUnlock, pickedFaction);
          FactionChoiceDialogPresenter.ReplaceRegionUnitsWithFactionEquivalents(Regions.Orgrimmar, pickedFaction);
          FactionChoiceDialogPresenter.ReplaceRegionUnitsWithFactionEquivalents(Regions.Crossroads, pickedFaction);
        }


        public override void OnNotPicked()
        {
            Regions.StonemaulKeep.CleanupNeutralPassiveUnits();
            base.OnNotPicked();
        }
        private void RegisterDialogue()
        {
            TriggeredDialogueManager.Add(new TriggeredDialogue(
              new Dialogue(@"Sound\Dialogue\OrcCampaign\Orc05\O05Grom26.flac",
                "Yes! I feel the power once again! Come, my warriors; drink from the dark waters, and you will be reborn!",
                "Grom Hellscream"), new[]
              {
          this
              }, new List<Objective>
              {
          new ObjectiveControlLegend(_allLegendSetup.Warsong.GromHellscream, false)
          {
            EligibleFactions = new List<Faction>
            {
              this
            }
          },
          new ObjectiveControlCapital(_allLegendSetup.Neutral.FountainOfBlood, false)
          {
            EligibleFactions = new List<Faction>
            {
              this
            }
          }
              }
            ));
            TriggeredDialogueManager.Add(
              new TriggeredDialogue(new DialogueSequence(new Dialogue(
                    @"Sound\Dialogue\OrcCampaign\Orc08\O08Grom33",
                    "Thrall... I see clearly now.  I'm... sorry.  I am so sorry..",
                    "Grom Hellscream"))
                , new[]
                {
            this
                }, new[]
                {
            new ObjectiveControlLegend(_allLegendSetup.Warsong.GromHellscream, false)
            {
              EligibleFactions = new List<Faction>{this}
            }
                }));
        }
        
        private void RegisterFlightPath()
        {
        
          ResearchManager.Register(new FlightPath(
            this,
            UPGRADE_R09N_FLIGHT_PATH_WARSONG,
            70,
            _preplacedUnitSystem));
        }
    }
}