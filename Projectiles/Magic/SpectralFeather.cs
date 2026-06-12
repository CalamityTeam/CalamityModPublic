using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SpectralFeather : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/Magic/StickyFeather";
        public Player Owner => Main.player[Projectile.owner];
        public bool visuals => Owner.Calamity().mageCrownVisibility; // Enables/disables visuals and sounds based on accessory visibility
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 0.8f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            bool isActive = Projectile.type == ModContent.ProjectileType<SpectralFeather>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.featherCrown)
            {
                modPlayer.mageCrownTimer = 0;
                modPlayer.mageCrownCount = 0;
                return;
            }
            if (isActive)
            {
                if (player.dead)
                {
                    modPlayer.featherCrown = false;
                }
                if (modPlayer.featherCrown)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.rotation += MathHelper.ToRadians(1.3f);

            int cap = modPlayer.mageCrownCount;
            if (Owner.ownedProjectileCounts[Type] > cap)
            {
                Projectile.Kill();
                return;
            }
            int feathers = 0;
            int featherAmt = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                {
                    if (Main.projectile[i] == Projectile)
                    {
                        feathers = featherAmt;
                    }
                    featherAmt++;
                }
            }
            float f = ((float)feathers / (float)featherAmt + player.miscCounterNormalized * 2f) * ((float)Math.PI * 2f);
            float num = 18f + (float)featherAmt;
            Vector2 vector = player.position - player.oldPosition;
            base.Projectile.Center += vector;
            Vector2 vector2 = f.ToRotationVector2();
            Projectile.localAI[0] = vector2.Y;
            Vector2 value = (player.Center + new Vector2(0f, -25f)) + vector2 * new Vector2(1f, 0.05f) * num;
            base.Projectile.Center = value;

            if (!Projectile.FinalExtraUpdate())
                return;

            // Give off some light
            float lightScalar = Main.rand.NextFloat(0.9f, 1.1f) * Main.essScale;
            Lighting.AddLight(Projectile.Center, 0.3f * lightScalar, 0.26f * lightScalar, 0.15f * lightScalar);

            Vector2 direction = player.Center - Projectile.Center;
            direction.Normalize();
            direction *= 6f;
            if (direction.X >= 0.25f)
            {
                Projectile.direction = -1;
            }
            else if (direction.X < -0.25f)
            {
                Projectile.direction = 1;
            }
            Projectile.spriteDirection = Projectile.direction;

        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (visuals)
            {
                SoundEngine.PlaySound(SoundID.Item20 with { volume = 0.6f }, player.Center);
            }
            if (modPlayer.mageCrownCount == 5)
            {
                SoundStyle max = new("CalamityMod/Sounds/Item/AscendantOff");
                SoundEngine.PlaySound(max with { volume = 0.6f }, player.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            float dustSp = 0.2f;
            int dustD = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Vector2 dustspeed = new Vector2(dustSp, dustSp).RotatedBy(MathHelper.ToRadians(dustD));
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, dustspeed.X, dustspeed.Y, 200, new Color(213, 242, 232, 200), 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center;
                    Main.dust[d].velocity = dustspeed;
                    dustSp += 0.2f;
                }
                dustD += 90;
                dustSp = 0.2f;
            }
            if (!Main.dedServ)
            {
                for (int i = 0; i < 4; i++)
                {
                    Particle dust = new GlowOrbParticle(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(2.5f, 3), false, 20, Main.rand.NextFloat(0.5f, 0.65f), Main.rand.NextBool(5) ? Color.AliceBlue : Color.DodgerBlue, true, false, false);
                    GeneralParticleHandler.SpawnParticle(dust);
                }
            }
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            //Ensures the breaking sound doesn't play when the player removes the accessory
            if (modPlayer.featherCrown)
            {
                SoundStyle f = new("CalamityMod/Sounds/Item/MeldShoot");
                SoundEngine.PlaySound(f with { Volume = 0.35f, Pitch = 0.95f }, Projectile.Center);
                SoundStyle aud = new("CalamityMod/Sounds/Item/MittFail");
                SoundEngine.PlaySound(aud with { Volume = 0.6f, Pitch = 0 }, Projectile.Center);
                SoundStyle shed = new("CalamityMod/Sounds/Item/FeatherBreak");
                SoundEngine.PlaySound(shed with { Volume = 1f, Pitch = 0 }, Projectile.Center);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.localAI[0] < 0)
            {
                overPlayers.Add(index);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.mageCrownCount == 5 && visuals)
            {
                for (int i = 0; i < 6; i++)
                {
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, (Color.Teal * 0.5f) with { A = 0 }, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.01f, SpriteEffects.None, 0f);
                }
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * (visuals ? 1f : 0.5f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
