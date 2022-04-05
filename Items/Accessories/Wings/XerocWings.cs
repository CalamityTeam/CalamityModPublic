using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class XerocWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exodus Wings");
            Tooltip.SetDefault("Pulsing with an alien heartbeat\n" +
                "Horizontal speed: 9.00\n" +
                "Acceleration multiplier: 2.5\n" +
                "Great vertical speed\n" +
                "Flight time: 180\n" +
                "5% increased rogue damage and critical strike chance while wearing the Empyrean Armor");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
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
                float xOffset = 4f;
                if (player.direction == 1)
                {
                    xOffset = -40f;
                }
                int index = Dust.NewDust(new Vector2(player.Center.X + xOffset, player.Center.Y - 15f), 30, 30, 62, 0f, 0f, 100, default, 2.4f);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[index].fadeIn = 2f;
                }
                Main.dust[index].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 180;
            player.noFallDmg = true;
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
            speed = 9f;
            acceleration *= 2.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MeldiateBar>(5)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
