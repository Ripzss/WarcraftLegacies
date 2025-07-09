﻿using System;
using MacroTools.SpellSystem;
using WCSharp.Buffs;
using WCSharp.Shared.Data;

namespace WarcraftLegacies.Source.Spells.WarpedMalediction
{
  public sealed class WarpedMalediction : Spell
  {
    private readonly struct BuffSpell
    {
      public readonly int AbilityId;
      public readonly int OrderId;
      public readonly string Name;

      public BuffSpell(int abilityId, int orderId, string name)
      {
        AbilityId = abilityId;
        OrderId = orderId;
        Name = name;
      }
    }

    private readonly BuffSpell[] _possibleBuffs;

    public WarpedMalediction(int id) : base(id)
    {
      _possibleBuffs = new[]
      {
        new BuffSpell(ABILITY_WMMF_MIND_FLAY_WARPED_MALEDICTION,ORDER_REJUVINATION, "Mind Flay"),
        new BuffSpell(ABILITY_WMSB_SOUL_BURN_WARPED_MALEDICTION, ORDER_SOUL_BURN, "Soul Burn"),
        new BuffSpell(ABILITY_WMCR_CURSE_WARPED_MALEDICTION, ORDER_CURSE, "Curse")
      };
    }

    public override void OnCast(unit caster, unit target, Point targetPoint)
    {
      try
      {
        if (!CastFilters.IsTargetEnemyAndAlive(caster, target))
        {
          return;
        }
        
        foreach (var buff in _possibleBuffs)
        {
          if (GetUnitAbilityLevel(target, buff.AbilityId) > 0)
          {
            return;
          }
        }

        var randomValue = GetRandomReal(0, 1);
        var randomIndex = (int)(randomValue * _possibleBuffs.Length);
        var selectedBuff = _possibleBuffs[randomIndex];

        BuffSystem.Add(new WarpedMaledictionBuff(caster, target, selectedBuff.AbilityId, selectedBuff.OrderId)
        {
          Active = true
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed to cast {nameof(WarpedMalediction)}: {ex}");
      }
    }
  }
}