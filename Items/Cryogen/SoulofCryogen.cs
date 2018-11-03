using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Cryogen
{
    [AutoloadEquip(EquipType.Wings)]
    public class SoulofCryogen : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of Cryogen");
            Tooltip.SetDefault("The magic of the ancient ice castle is yours\n" +
                "Counts as wings\n" +
                "Decent acceleration: 1.35\n" +
                "Decent flight time: 60\n" +
                "10% increase to all damage and pick speed\n" +
                "Frost damage added to melee swings");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 4));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 500000;
            item.expert = true;
            item.accessory = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            maxFallSpeed *= 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0.3f * num, 0.3f * num);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 8f;
            acceleration *= 1.35f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.cryogenSoul = true;
            player.pickSpeed -= 0.1f;
            player.magicDamage *= 1.1f;
            player.rangedDamage *= 1.1f;
            player.meleeDamage *= 1.1f;
            player.minionDamage *= 1.1f;
            player.thrownDamage *= 1.1f;
            player.wingTimeMax = 60;
        }
    }
}