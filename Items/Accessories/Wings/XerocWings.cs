using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class XerocWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Wings");
            Tooltip.SetDefault("Highly radioactive\n" +
                "Horizontal speed: 8.5\n" +
                "Acceleration multiplier: 2\n" +
                "Good vertical speed\n" +
                "Flight time: 180\n" +
                "5% increased rogue damage and critical strike chance while wearing the Xeroc Armor");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
            item.value = Item.buyPrice(0, 39, 99, 99);
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.armor[0].type == ModContent.ItemType<XerocMask>() && player.armor[1].type == ModContent.ItemType<XerocPlateMail>() && player.armor[2].type == ModContent.ItemType<XerocCuisses>())
            {
                player.Calamity().throwingDamage += 0.05f;
                player.Calamity().throwingCrit += 5;
            }

            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 242, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 180;
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 8.5f;
            acceleration *= 2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemID.SoulofFlight, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
