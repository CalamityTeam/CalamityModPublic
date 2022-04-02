using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoCooperLimbs : ModProjectile
    {
        private int AttackMode = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ascened Cooper");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.netImportant = true;
            projectile.width = 90;
            projectile.height = 90;
            projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile body = Main.projectile[(int)projectile.ai[1]];
            //Apply the buff
            bool flag64 = projectile.type == ModContent.ProjectileType<EndoCooperLimbs>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<EndoCooperBuff>(), 3600);

            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.endoCooper = false;
                }
                if (modPlayer.endoCooper)
                {
                    projectile.timeLeft = 2;
                }
            }

            //Spawn effects
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
                AttackMode = (int)projectile.ai[0];
                projectile.ai[0] = 0f;

                SpawnDust();
            }
            AttackMode = (int)body.localAI[1];

            //Rotation
            if (AttackMode == 3)
            {
                projectile.rotation += 0.015f;
            }
            else
            {
                float rotateratio = 0.007f;
                float rotation = (Math.Abs(body.velocity.X) + Math.Abs(body.velocity.Y)) * rotateratio;
                projectile.rotation += rotation * body.direction;
            }

            //Damage Update
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            //Keep the limbs in place

            if (body.type != ModContent.ProjectileType<EndoCooperBody>() || !body.active)
                projectile.Kill();
            projectile.Center = body.Center;

            //"Destroy" limbs
            if (projectile.ai[0] == 1f && AttackMode == 1)
            {
                projectile.alpha = 255;
                SpawnShards();
                projectile.ai[0] = 2f;
                projectile.netUpdate = true;
            }
            //Respawn limbs
            if (projectile.ai[0] == 3f)
            {
                SpawnDust();
                projectile.alpha = 255;
                projectile.ai[0] = 0f;
            }
            //Flames
            if (projectile.ai[0] == 4f)
            {
                SpawnFlames();
                projectile.ai[0] = 0f;
            }


        }

        public override void Kill(int timeLeft)
        {
            SpawnDust();
            Main.PlaySound(SoundID.NPCHit5, projectile.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] == 2f)
                return false;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] == 2f)
                return;
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Summon/EndoCooperLimbs_Glow"), projectile.Center - Main.screenPosition, frame, Color.LightSkyBlue, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override bool CanDamage()
        {
            return AttackMode == 2;
        }

        private void SpawnShards()
        {
            SpawnDust();
            Main.PlaySound(SoundID.NPCHit5, projectile.Center);
            if (AttackMode == 1)
            {
                for (int i = 0; i < 360; i += 60)
                {
                    float dmgmult = 0.8f;
                    Vector2 pspeed1 = new Vector2(Main.rand.NextFloat(3f, 8f), Main.rand.NextFloat(3f, 8f)).RotatedBy(MathHelper.ToRadians(i + 20 + MathHelper.ToDegrees(projectile.rotation)));
                    Projectile.NewProjectile(projectile.Center, pspeed1, ModContent.ProjectileType<EndoIceShard>(), (int)(projectile.damage * dmgmult), 1f, projectile.owner, 0f, 0f);
                    Vector2 pspeed2 = pspeed1.RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 13))) * Main.rand.NextFloat(0.6f, 1.3f);
                    Projectile.NewProjectile(projectile.Center, pspeed2, ModContent.ProjectileType<EndoIceShard>(), (int)(projectile.damage * dmgmult), 1f, projectile.owner, 0f, 0f);
                    Vector2 pspeed3 = pspeed1.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-12, -1))) * Main.rand.NextFloat(0.6f, 1.3f);
                    Projectile.NewProjectile(projectile.Center, pspeed3, ModContent.ProjectileType<EndoIceShard>(), (int)(projectile.damage * dmgmult), 1f, projectile.owner, 0f, 0f);
                }
            }
        }

        private void SpawnDust()
        {
            for (int i = 0; i < 50; i++)
            {
                int dusttype = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dusttype = 80;
                }
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, dusttype, dspeed.X, dspeed.Y, 50, default, 1.1f);
                Main.dust[dust].noGravity = true;
            }
        }

        private void SpawnFlames()
        {
            Main.PlaySound(SoundID.Item34, projectile.Center);

            for (int i = 0; i < 360; i += 60)
            {
                float dmgmult = 0.9f;
                Vector2 pspeed1 = new Vector2(0.9f, 0.9f).RotatedBy(MathHelper.ToRadians(i + 20 + MathHelper.ToDegrees(projectile.rotation)));
                int flame = Projectile.NewProjectile(projectile.Center, pspeed1, ModContent.ProjectileType<EndoFire>(), (int)(projectile.damage * dmgmult), 1f, projectile.owner, 0f, 0f);
            }
        }
    }
}
