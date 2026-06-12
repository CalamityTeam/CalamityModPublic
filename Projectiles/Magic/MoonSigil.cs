using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Melee;
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
    public class MoonSigil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public bool visuals => Owner.Calamity().mageCrownVisibility; // Enables/disables visuals and sounds based on accessory visibility
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.9f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            bool isActive = Projectile.type == ModContent.ProjectileType<MoonSigil>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.moonCrown)
            {
                modPlayer.mageCrownCount = 0;
                return;
            }
            if (isActive)
            {
                if (player.dead)
                {
                    modPlayer.moonCrown = false;
                }
                if (modPlayer.moonCrown)
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
            int sigils = 0;
            int sigilAmt = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                {
                    if (Main.projectile[i] == Projectile)
                    {
                        sigils = sigilAmt;
                    }
                    sigilAmt++;
                }
            }
            float f = ((float)sigils / (float)sigilAmt + player.miscCounterNormalized * 2f) * ((float)Math.PI * 2f);
            float num = 24f + (float)sigilAmt * 4.5f;
            Vector2 vector = player.position - player.oldPosition;
            base.Projectile.Center += vector;
            Vector2 vector2 = f.ToRotationVector2();
            Projectile.localAI[0] = vector2.Y;
            Vector2 value = (player.Center + new Vector2(0f, 5f)) + vector2 * new Vector2(1f, 0.05f) * num;
            base.Projectile.Center = value;

            if (!Projectile.FinalExtraUpdate())
                return;

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
                SoundEngine.PlaySound(SoundID.Item8, player.Center);
            }
            if (modPlayer.mageCrownCount == 10)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 1.3f }, player.Center);
            }
        }   
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            //Ensures the breaking sound doesn't play when the player removes the accessory
            if (modPlayer.moonCrown)
            {
                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack with { Volume = 0.4f, Pitch = 0.3f, MaxInstances = 1 }, player.Center);
                SoundEngine.PlaySound(Exoblade.BeamHitSound with { MaxInstances = 1 }, player.Center);
            }
            float dustSp = 0.9f;
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
                }
                dustD += 90;
            }
            for (int i = 0; i < 4; i++)
            {
                Color colorAccent = Main.rand.NextBool() ? Color.PaleTurquoise : Color.SeaGreen;

                Dust shatter = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail);
                shatter.velocity = Main.rand.NextVector2Circular(4.6f, 4.6f) + Projectile.velocity * 0.5f;
                shatter.color = Color.Lerp(Color.Turquoise, colorAccent, Main.rand.NextFloat(0.23f));
                shatter.scale *= 1.1f;
                shatter.fadeIn = 0.75f;
                shatter.noGravity = true;
            }
            int shardsplash = 0;
            while (shardsplash < 4)
            {
                GeneralParticleHandler.SpawnParticle(new PointParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(10), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Projectile.ai[0] == 0.6f ? 0.8f : 0.6f, Projectile.ai[0] == 1 ? Color.Turquoise : Color.LightSeaGreen, true, false));
                shardsplash += 1;
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
            if (modPlayer.mageCrownCount >= 10 && visuals)
            {
                for (int i = 0; i < 4; i++)
                {
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, (Color.Teal * 0.5f) with { A = 0 }, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
                }
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * (visuals ? 1f : 0.5f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
