using CalamityMod.Dusts;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class HadarianWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadarian Wings");
            Tooltip.SetDefault("Powered by the Astral Infection\n" +
                "Hold down to hover during flight, greatly extending flight duration\n" +
                "Horizontal speed: 9.00\n" +
                "Acceleration multiplier: 1.75\n" +
                "Good vertical speed\n" +
                "Flight time: 120\n" +
                "10% increased movement and jump speed while wearing the Astral Armor");
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(120, 9f, 1.75f);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.armor[0].type == ModContent.ItemType<AstralHelm>() && player.armor[1].type == ModContent.ItemType<AstralBreastplate>() && player.armor[2].type == ModContent.ItemType<AstralLeggings>())
            {
                player.moveSpeed += 0.1f;
                player.jumpSpeedBoost += 0.5f;
            }

            if (player.controlJump && player.wingTime > 0f && !player.canJumpAgain_Cloud && player.jump == 0)
            {
                if (player.controlDown && !player.merman)
                {
                    player.velocity.Y *= 0.9f;
                    if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    {
                        player.velocity.Y = 1E-05f;
                    }
                    player.wingTime += 0.75f;
                }
                if (player.velocity.Y != 0f && !hideVisual)
                {
                    int num59 = 4;
                    if (player.direction == 1)
                    {
                        num59 = -40;
                    }
                    int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.75f);
                    Main.dust[num60].noGravity = true;
                    Main.dust[num60].velocity *= 0.3f;
                    if (Main.rand.NextBool(10))
                    {
                        Main.dust[num60].fadeIn = 2f;
                    }
                    Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                }
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.8f;
            ascentWhenRising = 0.155f;
            maxCanAscendMultiplier = 1.05f;
            maxAscentMultiplier = 2.55f;
            constantAscend = 0.13f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(8).
                AddIngredient<HadarianMembrane>(8).
                AddIngredient(ItemID.SoulofFlight, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
