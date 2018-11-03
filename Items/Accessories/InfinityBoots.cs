using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings, EquipType.Shoes)]
    public class InfinityBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seraph Tracers");
            Tooltip.SetDefault("Counts as wings\n" +
                "Excellent acceleration: 2.25\n" +
                "Excellent flight time: 120\n" +
                "Ludicrous speed!\n" +
                "Greater mobility on ice\n" +
                "Water and lava walking\n" +
                "Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = 5000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 107, 0f, 0f, 100, default(Color), 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.Next(10) == 0)
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            player.accRunSpeed = 9.25f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.24f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 920;
            player.wingTimeMax = 120;
            modPlayer.IBoots = !hideVisual;
            modPlayer.sTracers = true;
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
            speed = 10f;
            acceleration *= 2.25f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("WingsGroup");
            recipe.AddIngredient(null, "AngelTreads");
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}