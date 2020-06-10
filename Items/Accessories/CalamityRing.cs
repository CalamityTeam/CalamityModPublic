using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CalamityRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void of Calamity");
            Tooltip.SetDefault("Cursed?\n" +
			"15% increase to all damage\n" +
			"Brimstone fire rains down while invincibility is active");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 9;
            item.accessory = true;
            item.expert = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().calamityRing;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.calamityRing = true;
            player.allDamage += 0.15f;
            player.endurance -= 0.15f;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.immune)
                {
                    if (Main.rand.NextBool(10))
                    {
						float x = player.position.X + (float)Main.rand.Next(-400, 400);
						float y = player.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 source = new Vector2(x, y);
						Vector2 velocity = player.Center - source;
						velocity.X += (float)Main.rand.Next(-100, 101);
						float speed = 22f;
						float targetDist = velocity.Length();
						targetDist = speed / targetDist;
						velocity.X *= targetDist;
						velocity.Y *= targetDist;
						int fire = Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<StandingFire>(), (int)(30 * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
						Main.projectile[fire].ai[1] = player.position.Y;
                    }
                }
            }
        }
    }
}
