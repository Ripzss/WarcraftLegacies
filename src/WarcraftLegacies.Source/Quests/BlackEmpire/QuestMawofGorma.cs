﻿using System.Collections.Generic;
using MacroTools.Extensions;
using MacroTools.FactionSystem;
using MacroTools.ObjectiveSystem.Objectives.ControlPointBased;
using MacroTools.ObjectiveSystem.Objectives.FactionBased;
using MacroTools.ObjectiveSystem.Objectives.TimeBased;
using MacroTools.QuestSystem;
using WCSharp.Shared.Data;

namespace WarcraftLegacies.Source.Quests.BlackEmpire
{ 
  /// <summary>
  /// Kill some creeps to gain an outpost and 1 forgotten one.
  /// </summary>
  public sealed class QuestMawofGorma : QuestData
  {
    private readonly List<unit> _rescueUnits;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestMawofGorma"/> class.
    /// </summary>
    /// <param name="rescueRect">Units in this area will start invulnerable and be rescued when the quest is complete.</param>
    public QuestMawofGorma(Rectangle rescueRect) : base("Maw of Gor'ma",
      "Invaders from Azeroth have taken control of the Maw of Gor'ma. Destroy them!",
      @"ReplaceableTextures\CommandButtons\BTNForgottenOne.blp")
    {
      AddObjective(new ObjectiveControlPoint(UNIT_NMOG_MAW_OF_GOR_MA));
      AddObjective(new ObjectiveExpire(660, Title));
      AddObjective(new ObjectiveSelfExists());
      _rescueUnits = rescueRect.PrepareUnitsForRescue(RescuePreparationMode.HideAll,
        filterUnit => filterUnit.GetTypeId() != FourCC("ngol"));
      ResearchId = UPGRADE_RBMG_QUEST_COMPLETED_MAW_OF_GOR_MA;

    }

    /// <inheritdoc />
    public override string RewardFlavour => "With the invaders defeated, I have retaken control of the Maw of Gor'ma.";

    /// <inheritdoc />
    protected override string RewardDescription => $"Gain Control of all buildings in the Maw of Gor'ma area, learn to train X'korr the Compelling from the {GetObjectName(UNIT_N0AV_ALTAR_OF_MADNESS_YOGG_ALTAR)} and the ability to train 1 {GetObjectName(UNIT_U02F_FORGOTTEN_ONE_YOGG)} from the {GetObjectName(UNIT_N0AX_MUTATION_CIRCLE_YOGG_SPECIALIST)}";

    /// <inheritdoc />
    protected override void OnFail(Faction completingFaction)
    {
      var rescuer = completingFaction.ScoreStatus == ScoreStatus.Defeated
        ? Player(PLAYER_NEUTRAL_AGGRESSIVE)
        : completingFaction.Player;

      rescuer.RescueGroup(_rescueUnits);
    }

    /// <inheritdoc />
    protected override void OnComplete(Faction completingFaction) => 
      completingFaction.Player.RescueGroup(_rescueUnits);
  }
}