using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class YharimsGift : ModItem
    {
        public int dragonTimer = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Gift");
            Tooltip.SetDefault("The power of a god pulses from within this artifact\n" +
                               "Flaming meteors rain down while invincibility is active\n" +
                               "Exploding dragon dust is left behind as you move\n" +
                               "Defense increased by 30 and damage increased by 15%");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.accessory = true;
            item.expert = true;
            item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.allDamage += 0.15f;
            player.statDefense += 30;
            if (!player.StandingStill())
            {
                dragonTimer--;
                if (dragonTimer <= 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int projectile1 = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), (int)(350 * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
                        Main.projectile[projectile1].timeLeft = 60;
                    }
                    dragonTimer = 60;
                }
            }
            else
            {
                dragonTimer = 60;
            }
            if (player.immune)
            {
                if (Main.rand.NextBool(8))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<SkyFlareFriendly>(), (int)(750 * player.AverageDamage()), 9f, player.whoAmI);
                    }
                }
            }
        }
    }
}
