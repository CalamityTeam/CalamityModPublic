using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class MOAB : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MOAB");
            Tooltip.SetDefault("The mother of all balloons\n" +
                "Counts as wings\n" +
                "Horizontal speed: 6.50\n" +
                "Acceleration multiplier: 1.0\n" +
                "Good vertical speed\n" +
                "Flight time: 75\n" +
                "10% increased jump speed and allows constant jumping\n" +
                "Grants the player cloud, blizzard, and sandstorm mid-air jumps");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity6BuyPrice;
            item.rare = ItemRarityID.LightPurple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                player.rocketDelay2--;
                if (player.rocketDelay2 <= 0)
                {
                    Main.PlaySound(SoundID.Item13, player.position);
                    player.rocketDelay2 = 60;
                }
                int dustAmt = 2;
                if (player.controlUp)
                {
                    dustAmt = 4;
                }
                for (int index = 0; index < dustAmt; index++)
                {
                    int type = DustID.Fire;
                    if (player.head == 41)
                    {
                        int arg_58FD_0 = player.body;
                    }
                    float scale = 1.75f;
                    int alpha = 100;
                    float xStart = player.Center.X + 16f;
                    if (player.direction > 0)
                    {
                        xStart = player.Center.X - 26f;
                    }
                    float yStart = player.position.Y + (float)player.height - 18f;
                    if (index == 1 || index == 3)
                    {
                        xStart = player.Center.X + 8f;
                        if (player.direction > 0)
                        {
                            xStart = player.Center.X - 20f;
                        }
                        yStart += 6f;
                    }
                    if (index > 1)
                    {
                        yStart += player.velocity.Y;
                    }
                    int num69 = Dust.NewDust(new Vector2(xStart, yStart), 8, 8, type, 0f, 0f, alpha, default, scale);
                    Dust dust = Main.dust[num69];
                    dust.velocity.X *= 0.1f;
                    dust.velocity.Y = Main.dust[num69].velocity.Y * 1f + 2f * player.gravDir - player.velocity.Y * 0.3f;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                    if (dustAmt == 4)
                    {
                        dust.velocity.Y += 6f;
                    }
                }
            }
            player.doubleJumpCloud = true;
            player.doubleJumpSandstorm = true;
            player.doubleJumpBlizzard = true;
            player.jumpBoost = true;
            player.autoJump = true;
            player.noFallDmg = true;
            player.jumpSpeedBoost += 0.5f;
            player.wingTimeMax = 75;
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
            speed = 6.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrogLeg);
            recipe.AddIngredient(ItemID.BundleofBalloons);
            recipe.AddIngredient(ItemID.LuckyHorseshoe);
            recipe.AddIngredient(ItemID.Jetpack);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
