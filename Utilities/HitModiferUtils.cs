using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.WorldBuilding;

namespace CalamityMod.Utilities
{
    internal static class HitModiferUtils
    {
        extension(ref NPC.HitModifiers modifiers)
        {
            internal void ApplyScalingForcedCrit(Projectile Projectile)
            {
                modifiers.SetCrit();
                float critDamage = Main.player[Projectile.owner].GetTotalCritChance(Projectile.DamageType) * 0.02f;
                modifiers.CritDamage += critDamage;
            }
        }
    }
}
