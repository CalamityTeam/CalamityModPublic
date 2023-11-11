using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceCrystalShard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool healerGuardianAlive = true;
            if (CalamityGlobalNPC.doughnutBossHealer < 0 || !Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                healerGuardianAlive = false;

            Lighting.AddLight(Projectile.Center, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity);

            // Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;
            
            // Night AI or Guardian Healer
            if (Projectile.maxPenetrate != (int)Providence.BossMode.Day || healerGuardianAlive)
                Projectile.extraUpdates = 1;

            if (Projectile.timeLeft < 300)
                Projectile.tileCollide = true;

            Color newColor2 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);

            if (Projectile.Opacity < 1f)
                Projectile.Opacity += 0.03f;

            if (Projectile.Opacity > 1f)
                Projectile.Opacity = 1f;

            if (Projectile.Opacity == 1f)
                Lighting.AddLight(Projectile.Center, newColor2.ToVector3() * 0.5f);

            Projectile.velocity.X *= 0.995f;
            if (Projectile.velocity.Y < 0f)
            {
                Projectile.velocity.Y *= 0.98f;
            }
            else
            {
                Projectile.velocity.Y *= 1.06f;
                float fallSpeed = (CalamityWorld.revenge || (Projectile.maxPenetrate != (int)Providence.BossMode.Day)) ? 3.5f : 3f;
                if (Projectile.velocity.Y > fallSpeed)
                    Projectile.velocity.Y = fallSpeed;
            }

            if (Projectile.velocity.Y > -0.5f && Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                Projectile.velocity.Y = 0.5f;
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustRotation = Vector2.UnitY.RotatedBy(i * MathHelper.Pi).RotatedBy(Projectile.rotation);
                    Dust crystalDust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    crystalDust.noGravity = true;
                    crystalDust.noLight = true;
                    crystalDust.scale = Projectile.Opacity * Projectile.localAI[0];
                    crystalDust.position = Projectile.Center;
                    crystalDust.velocity = dustRotation;
                }
            }

            for (int j = 0; j < 2; j++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustRotate = Vector2.UnitY.RotatedBy(j * MathHelper.Pi);
                    Dust crystalDust2 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    crystalDust2.noGravity = true;
                    crystalDust2.noLight = true;
                    crystalDust2.scale = Projectile.Opacity * Projectile.localAI[0];
                    crystalDust2.position = Projectile.Center;
                    crystalDust2.velocity = dustRotate;
                }
            }

            if (Main.rand.NextBool(10))
            {
                float dustVelScale = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float dustScale = 1f + Main.rand.NextFloat();
                Vector2 randomDustOffset = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (randomDustOffset != Vector2.Zero)
                {
                    randomDustOffset.Normalize();
                }
                randomDustOffset *= 16f + Main.rand.NextFloat() * 16f;
                Vector2 dustPos = Projectile.Center + randomDustOffset;
                Point dustTileCoords = dustPos.ToTileCoordinates();
                bool shouldSpawn = true;
                if (!WorldGen.InWorld(dustTileCoords.X, dustTileCoords.Y, 0))
                {
                    shouldSpawn = false;
                }
                if (shouldSpawn && WorldGen.SolidTile(dustTileCoords.X, dustTileCoords.Y))
                {
                    shouldSpawn = false;
                }
                if (shouldSpawn)
                {
                    Dust holyDust = Main.dust[Dust.NewDust(dustPos, 0, 0, 267, 0f, 0f, 127, newColor2, 1f)];
                    holyDust.noGravity = true;
                    holyDust.position = dustPos;
                    holyDust.velocity = -Vector2.UnitY * dustVelScale * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    holyDust.fadeIn = fadeIn;
                    holyDust.scale = dustScale;
                    holyDust.noLight = true;
                    Dust holyDust2 = Dust.CloneDust(holyDust);
                    Dust dust = holyDust2;
                    dust.scale *= 0.65f;
                    dust = holyDust2;
                    dust.fadeIn *= 0.65f;
                    holyDust2.color = new Color(255, 255, 255, 255);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
            float dustAmt = Main.rand.Next(7, 13);
            Vector2 randomDustVelMod = new Vector2(1.6f, 1.5f);
            Color newColor = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            for (float i = 0f; i < dustAmt; i++)
            {
                int killDust = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[killDust].position = Projectile.Center;
                Main.dust[killDust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / dustAmt) * randomDustVelMod * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].scale = 2f;
                Main.dust[killDust].fadeIn = Main.rand.NextFloat() * 2f;
                Dust killDustClone = Dust.CloneDust(killDust);
                Dust dust = killDustClone;
                dust.scale /= 2f;
                dust = killDustClone;
                dust.fadeIn /= 2f;
                killDustClone.color = new Color(255, 255, 255, 255);
            }
            for (float j = 0f; j < dustAmt; j++)
            {
                int killDust2 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[killDust2].position = Projectile.Center;
                Main.dust[killDust2].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * j / dustAmt) * randomDustVelMod * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[killDust2];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat() * 1f;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust killDustClone2 = Dust.CloneDust(killDust2);
                dust = killDustClone2;
                dust.scale /= 2f;
                dust = killDustClone2;
                dust.fadeIn /= 2f;
                killDustClone2.color = new Color(255, 255, 255, 255);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 * Projectile.Opacity, 255 * Projectile.Opacity, 255 * Projectile.Opacity, 0);

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                modifiers.SourceDamage *= 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if ((info.Damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red) || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 0, 10);
        }
    }
}
