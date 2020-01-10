using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrittleStarMinion : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brittle Star");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 66;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            aiType = 388;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (dust == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 33, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                dust += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }
            projectile.rotation += projectile.velocity.X * 0.04f;
            bool flag64 = projectile.type == ModContent.ProjectileType<BrittleStarMinion>();
            player.AddBuff(ModContent.BuffType<BrittleStar>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.bStar = false;
                }
                if (modPlayer.bStar)
                {
                    projectile.timeLeft = 2;
                }
            }
            float SAImovement = 0.05f;
            for (int num638 = 0; num638 < Main.projectile.Length; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<BrittleStarMinion>();
                if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num638].position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < Main.projectile[num638].position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }
    }
}
