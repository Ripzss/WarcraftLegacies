﻿using System.Collections.Generic;
using System.Linq;
using MacroTools.Extensions;
using WCSharp.Buffs;
using WCSharp.Shared.Data;

using Environment = MacroTools.Libraries.Environment;

namespace MacroTools.Buffs
{
  public sealed class DelayedRecallBuff : PassiveBuff
  {
    /// <summary>
    /// The units that should be moved when the buff is disposed off.
    /// </summary>
    private List<unit> UnitsToMove { get;}
    
    /// <summary>
    /// The point of the casting unit when first cast.
    /// </summary>
    private Point TargetPosition { get; }
    
    /// <summary>The percentage of units to lose when the caster dies (rounded down)</summary>
    public float DeathPenalty { get; init; }
    
    /// <summary>
    /// The effect that is created when the buff is applied.
    /// </summary>
    private new effect? Effect { get; set; }

    private effect? _progressEffect;

    /// <inheritdoc />
    public DelayedRecallBuff(unit caster, unit target, List<unit> unitsToMove) : base(caster, target)
    {
      UnitsToMove = unitsToMove;
      TargetPosition =  new Point(GetUnitX(caster), GetUnitY(caster));
    }
    
    /// <inheritdoc />
    public override void OnApply()
    {
      foreach (var unit in UnitsToMove)
      {
        unit.IsVisible = false;
        unit.IsInvulnerable = true;
      }

      Effect = AddSpecialEffect(@"Abilities\Spells\Undead\Darksummoning\DarkSummonTarget.mdl", TargetPosition.X,
        TargetPosition.Y);
      
      _progressEffect = AddSpecialEffect("war3mapImported\\Progressbar10sec.mdx", TargetPosition.X, TargetPosition.Y);
      _progressEffect.SetTimeScale(10 / Duration);
      _progressEffect.SetTimeScale(10 / Duration);
      _progressEffect.SetColor(Caster.Owner);
      _progressEffect.SetHeight(185f + Environment.GetPositionZ(TargetPosition));
    }

    /// <inheritdoc />
    public override void OnDispose()
    {
      effect tempQualifier = Effect;
      if (tempQualifier != null)
      {
        tempQualifier.Dispose();
      }

      effect tempQualifier1 = _progressEffect;
      if (tempQualifier1 != null)
      {
        tempQualifier1.Dispose();
      }

      if (!UnitAlive(Caster))
      {
        var amountToKill = (int)(UnitsToMove.Count * DeathPenalty);
        foreach (var unit in UnitsToMove.Take(amountToKill)) 
          unit.Kill();
      }
      
      foreach (var unit in UnitsToMove)
      {
        unit.IsVisible = true;
        unit.SetPosition(TargetPosition);
        unit.IsInvulnerable = true;
      }
    }
  }
}