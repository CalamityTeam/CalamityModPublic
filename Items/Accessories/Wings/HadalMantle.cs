using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class HadalMantle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hadal Mantle");
            Tooltip.SetDefault("Fueled by the fury of the depths\n" +
                "Horizontal speed: 7.75\n" +
                "Acceleration multiplier: 1.5\n" +
                "Average vertical speed\n" +
                "Flight time: 180\n" +
                "5% increased damage while wearing the Hydrothermic Armor");
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 7.75f, 1.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<AtaxiaHeadgear>() || player.armor[0].type == ModContent.ItemType<AtaxiaHelm>() ||
                player.armor[0].type == ModContent.ItemType<AtaxiaHelmet>() || player.armor[0].type == ModContent.ItemType<AtaxiaHood>() ||
                player.armor[0].type == ModContent.ItemType<AtaxiaMask>()) &&
                player.armor[1].type == ModContent.ItemType<AtaxiaArmor>() && player.armor[2].type == ModContent.ItemType<AtaxiaSubligar>())
            {
                player.GetDamage<GenericDamageClass>() += 0.05f;
            }

            if (player.controlJump && player.wingTime > 0f && !player.canJumpAgain_Cloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 6, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.6f;
            ascentWhenRising = 0.12f;
            maxCanAscendMultiplier = 0.7f;
            maxAscentMultiplier = 1.75f;
            constantAscend = 0.11f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CruptixBar>(5).
                AddIngredient<CoreofChaos>().
                AddIngredient(ItemID.SoulofFlight, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
