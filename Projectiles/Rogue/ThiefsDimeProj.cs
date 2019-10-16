using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ThiefsDimeProj : ModProjectile
    {
        private double rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thief's Dime");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.Calamity().rogue = true;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }
            bool flag64 = projectile.type == mod.ProjectileType("ThiefsDimeProj");
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.thiefsDime)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.tDime = false;
                }
                if (modPlayer.tDime)
                {
                    projectile.timeLeft = 2;
                }
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            Vector2 vector = player.Center - projectile.Center;
            projectile.rotation = vector.ToRotation() - 1.57f;
            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Midas, 360);
            Player player = Main.player[projectile.owner];
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
			{
				if (Main.rand.NextBool(5))
				{
					Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.SilverCoin, Main.rand.Next(10, 21));
				}
				if (Main.rand.NextBool(10))
				{
					Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.GoldCoin);
				}
			}
			else
			{
				return;
			}
        }
    }
}
