using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TerminusHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public const int Lifetime = 300;
        public override string Texture => "CalamityMod/Items/SummonItems/BossRush";
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = Lifetime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Time++;
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                // Reset the boss rush timer to what it would normally be if disabling is done prematurely.
                if (BossRushEvent.BossRushActive || BossRushEvent.StartTimer > 0)
                    BossRushEvent.SyncStartTimer(BossRushEvent.StartEffectTotalTime);

                CreateMysticDeathDust();
                projectile.Kill();
                return;
            }

            UpdatePlayerFields();
            if (BossRushEvent.BossRushActive || BossRushEvent.StartTimer > 0)
            {
                float lifetime = Utils.InverseLerp(0f, 30f, Time, true);
                if (Time % 5f == 4f)
                    BossRushEvent.SyncStartTimer((int)MathHelper.Lerp(0f, BossRushEvent.StartEffectTotalTime, 1f - lifetime));

                MoonlordDeathDrama.RequestLight(Utils.InverseLerp(0f, 15f, Time, true), Main.LocalPlayer.Center);
                if (Time >= 45f)
                {
                    Main.PlaySound(SoundID.DD2_EtherianPortalOpen, Main.LocalPlayer.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                       BossRushEvent.End();

                    projectile.Kill();
                }

                return;
            }

            if (projectile.timeLeft == 1)
            {
                projectile.Kill();
                CreateEffectsHandler();
                return;
            }

            CreateIdleMagicDust();
            float currentShakePower = MathHelper.Lerp(0.2f, 8f, Utils.InverseLerp(Lifetime * 0.725f, Lifetime, Time, true));
            currentShakePower *= 1f - Utils.InverseLerp(1000f, 3100f, Main.LocalPlayer.Distance(projectile.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = currentShakePower;
        }

        public void CreateEffectsHandler()
        {
            Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, Main.LocalPlayer.Center);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = 16f;
            if (Main.myPlayer == projectile.owner)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BossRushEffectThing>(), 0, 0f, projectile.owner);
        }

        public void CreateMysticDeathDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 20; i++)
            {
                Dust paleMagic = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(22f, 28f), 261);
                paleMagic.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.8f, 3.2f);
                paleMagic.color = Color.White;
                paleMagic.scale = Main.rand.NextFloat(1.1f, 1.35f);
                paleMagic.fadeIn = 1.5f;
                paleMagic.noGravity = true;
            }
        }

        public void CreateIdleMagicDust()
        {
            if (Main.dedServ)
                return;

            int dustCount = (int)Math.Round(MathHelper.SmoothStep(1f, 5f, Time / Lifetime));
            float outwardness = MathHelper.SmoothStep(40f, 150f, Time / Lifetime);
            float dustScale = MathHelper.Lerp(1.15f, 1.725f, Time / Lifetime);
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2Unit() * outwardness * Main.rand.NextFloat(0.75f, 1.1f);
                Vector2 dustVelocity = (projectile.Center - spawnPosition) * 0.085f + Owner.velocity;

                Dust paleMagic = Dust.NewDustPerfect(spawnPosition, 264);
                paleMagic.velocity = dustVelocity;
                paleMagic.scale = dustScale * Main.rand.NextFloat(0.75f, 1.15f);
                paleMagic.color = Color.Lerp(Color.LightCoral, Color.White, Time / Lifetime * Main.rand.NextFloat(0.65f, 1f));
                paleMagic.noGravity = true;
                paleMagic.noLight = true;
            }
        }

        public void UpdatePlayerFields()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.spriteDirection = Owner.direction;
                projectile.localAI[0] = 1f;
            }
            Owner.itemRotation = 0f;
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(projectile.spriteDirection);

            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + Vector2.UnitX * projectile.spriteDirection * 26f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 baseDrawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            Color baseColor = Color.Lerp(projectile.GetAlpha(lightColor), Color.White, Utils.InverseLerp(40f, 120f, Time, true));
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Time >= 150f)
            {
                float outwardness = MathHelper.SmoothStep(1f, 15f, Utils.InverseLerp(150f, Lifetime - 30f, Time, true));
                Color afterimageColor = Color.Lerp(baseColor, Color.LightCoral, Utils.InverseLerp(150f, 195f, Time, true)) * 0.225f;
                afterimageColor.A = 0;

                for (int i = 0; i < 10; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 10f + Main.GlobalTime * 4.4f).ToRotationVector2() * outwardness;
                    spriteBatch.Draw(texture, baseDrawPosition + drawOffset, null, afterimageColor, 0f, origin, projectile.scale, direction, 0f);
                }
            }
            spriteBatch.Draw(texture, baseDrawPosition, null, baseColor, 0f, origin, projectile.scale, direction, 0f);

            return false;
        }
    }
}
