using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public abstract class CalamityDamageItem : ModItem
    {
        public virtual void SafeSetDefaults()
        {
        }

        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            item.melee = false;
            item.ranged = true;
            item.magic = false;
            item.thrown = false;
            item.summon = false;
        }

        public override void GetWeaponDamage(Player player, ref int damage)
        {
            //damage = (int)((float)damage / (player.rangedDamage + 5E-06f)); //change base damage back to normal after all ranged boosts
            damage = (int)((double)item.damage * (double)(CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage + 5E-06f)); //plus one otherwise weird shit happens
        }

        public override void GetWeaponCrit(Player player, ref int crit)
        {
            crit = (item.crit + CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit); //change crit back to normal
        }

        public override void GetWeaponKnockback(Player player, ref float knockback)
        {
            if (player.shroomiteStealth)
            {
                knockback /= 1f + (1f - player.stealth) * 0.5f;
            }
            if (player.setVortex)
            {
                knockback /= 1f + (1f - player.stealth) * 0.5f;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
            if (tt != null)
            {
                string[] splitText = tt.text.Split(' ');
                string damageValue = splitText.First();
                string damageWord = splitText.Last();
                tt.text = damageValue + " rogue " + damageWord;
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
            if (tt2 != null)
            {
                if (CalamityMod.lightGreenThrowingRarityList.Contains(item.type))
                    tt2.overrideColor = new Color(0, 255, 200);
                else if (CalamityMod.greenThrowingRarityList.Contains(item.type))
                    tt2.overrideColor = new Color(0, 255, 0);
                else if (CalamityMod.blueThrowingRarityList.Contains(item.type))
                    tt2.overrideColor = new Color(43, 96, 222);
                else if (item.type == mod.ItemType("Celestus"))
                    tt2.overrideColor = new Color(108, 45, 199);
                else if (CalamityMod.purpleThrowingRarityList.Contains(item.type))
                    tt2.overrideColor = new Color(255, 0, 255);
                else if (item.type == mod.ItemType("Malachite"))
                    tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
            }
        }

        public override bool ConsumeItem(Player player)
        {
            if (CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingAmmoCost50)
            {
                if (Main.rand.Next(1, 101) > 50)
                    return false;
            }
            if (CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingAmmoCost66)
            {
                if (Main.rand.Next(1, 101) > 66)
                    return false;
            }
            return true;
        }
    }
}