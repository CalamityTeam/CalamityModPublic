using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoCooperLimbs : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int AttackMode = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.netImportant = true;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile body = Main.projectile[(int)Projectile.ai[1]];
            //Apply the buff
            bool isMinion = Projectile.type == ModContent.ProjectileType<EndoCooperLimbs>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<EndoCooperBuff>(), 3600);

            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.endoCooper = false;
                }
                if (modPlayer.endoCooper)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //Spawn effects
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] += 1f;
                AttackMode = (int)Projectile.ai[0];
                Projectile.ai[0] = 0f;

                SpawnDust();
            }
            AttackMode = (int)body.localAI[1];

            //Rotation
            if (AttackMode == 3)
            {
                Projectile.rotation += 0.015f;
            }
            else
            {
                float rotateratio = 0.007f;
                float rotation = (Math.Abs(body.velocity.X) + Math.Abs(body.velocity.Y)) * rotateratio;
                Projectile.rotation += rotation * body.direction;
            }

            //Keep the limbs in place

            if (body.type != ModContent.ProjectileType<EndoCooperBody>() || !body.active)
                Projectile.Kill();
            Projectile.Center = body.Center;

            //"Destroy" limbs
            if (Projectile.ai[0] == 1f && AttackMode == 1)
            {
                Projectile.alpha = 255;
                SpawnShards();
                Projectile.ai[0] = 2f;
                Projectile.netUpdate = true;
            }
            //Respawn limbs
            if (Projectile.ai[0] == 3f)
            {
                SpawnDust();
                Projectile.alpha = 255;
                Projectile.ai[0] = 0f;
            }
            //Flames
            if (Projectile.ai[0] == 4f)
            {
                SpawnFlames();
                Projectile.ai[0] = 0f;
            }


        }

        public override void OnKill(int timeLeft)
        {
            SpawnDust();
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 2f)
                return false;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {            
            if (Projectile.ai[0] == 2f)
                return;

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/EndoCooperLimbs_Glow").Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.LightSkyBlue, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override bool? CanDamage() => AttackMode == 2;

        private void SpawnShards()
        {
            SpawnDust();
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.Center);
            if (AttackMode == 1)
            {
                for (int i = 0; i < 360; i += 60)
                {
                    Vector2 pspeed1 = new Vector2(Main.rand.NextFloat(3f, 8f), Main.rand.NextFloat(3f, 8f)).RotatedBy(MathHelper.ToRadians(i + 20 + MathHelper.ToDegrees(Projectile.rotation)));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, pspeed1, ModContent.ProjectileType<EndoIceShard>(), Projectile.damage, 1f, Projectile.owner, 0f, 0f);
                    Vector2 pspeed2 = pspeed1.RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 13))) * Main.rand.NextFloat(0.6f, 1.3f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, pspeed2, ModContent.ProjectileType<EndoIceShard>(), Projectile.damage, 1f, Projectile.owner, 0f, 0f);
                    Vector2 pspeed3 = pspeed1.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-12, -1))) * Main.rand.NextFloat(0.6f, 1.3f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, pspeed3, ModContent.ProjectileType<EndoIceShard>(), Projectile.damage, 1f, Projectile.owner, 0f, 0f);
                }
            }
        }

        private void SpawnDust()
        {
            for (int i = 0; i < 50; i++)
            {
                int dusttype = Main.rand.NextBool() ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dusttype = 80;
                }
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dusttype, dspeed.X, dspeed.Y, 50, default, 1.1f);
                Main.dust[dust].noGravity = true;
            }
        }

        private void SpawnFlames()
        {
            SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);

            for (int i = 0; i < 360; i += 60)
            {
                Vector2 pspeed1 = new Vector2(0.9f, 0.9f).RotatedBy(MathHelper.ToRadians(i + 20 + MathHelper.ToDegrees(Projectile.rotation)));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, pspeed1, ModContent.ProjectileType<EndoFire>(), Projectile.damage, 1f, Projectile.owner, 0f, 0f);
            }
        }
    }
}
