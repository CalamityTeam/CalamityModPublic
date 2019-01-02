using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Buffs
{
	public class CalamityGlobalBuff : GlobalBuff
	{
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Shine)
            {
                player.GetModPlayer<CalamityPlayer>(mod).shine = true;
            }
            if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
            if (type == BuffID.Rage)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
            }
            if (type == BuffID.Wrath)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
            }
            if (type == BuffID.WellFed)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            if (type >= BuffID.NebulaUpDmg1 && type <= BuffID.NebulaUpDmg3)
            {
                float nebulaDamage = 0.15f * (float)player.nebulaLevelDamage;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += nebulaDamage;
            }
            if (type == BuffID.Warmth)
            {
                player.buffImmune[mod.BuffType("GlacialState")] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Chilled] = true;
            }
        }
    }
}
