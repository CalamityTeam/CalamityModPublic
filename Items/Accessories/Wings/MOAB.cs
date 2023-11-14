using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class MOAB : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(75, 6.5f, 1f);
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                player.rocketDelay2--;
                if (player.rocketDelay2 <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item13, player.Center);
                    player.rocketDelay2 = 60;
                }
                int dustAmt = 2;
                if (player.controlUp)
                {
                    dustAmt = 4;
                }
                for (int index = 0; index < dustAmt; index++)
                {
                    int type = 6;
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
                    int boosterDust = Dust.NewDust(new Vector2(xStart, yStart), 8, 8, type, 0f, 0f, alpha, default, scale);
                    Dust dust = Main.dust[boosterDust];
                    dust.velocity.X *= 0.1f;
                    dust.velocity.Y = Main.dust[boosterDust].velocity.Y * 1f + 2f * player.gravDir - player.velocity.Y * 0.3f;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                    if (dustAmt == 4)
                    {
                        dust.velocity.Y += 6f;
                    }
                }
            }

            // Grants Cloud in a Bottle, Sandstorm in a Bottle, and Blizzard in a Bottle (like Bundle of Balloons)
            player.GetJumpState(ExtraJump.CloudInABottle).Enable();
            player.GetJumpState(ExtraJump.SandstormInABottle).Enable();
            player.GetJumpState(ExtraJump.BlizzardInABottle).Enable();
            player.jumpBoost = true;
            player.autoJump = true;
            player.noFallDmg = true;
            player.jumpSpeedBoost += 0.5f;
            player.luck += 0.05f;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FrogLeg).
                AddIngredient(ItemID.HorseshoeBundle).
                AddIngredient(ItemID.Jetpack).
                AddIngredient(ItemID.SoulofFright).
                AddIngredient(ItemID.SoulofMight).
                AddIngredient(ItemID.SoulofSight).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
