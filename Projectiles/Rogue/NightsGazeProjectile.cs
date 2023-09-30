using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class NightsGazeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/NightsGaze";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        private int SplitProjDamage => (int)(Projectile.damage * 0.6f);

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.ToRadians(45);

            if (Projectile.Calamity().stealthStrike)
            {
                if (Main.rand.NextBool(8))
                {
                    int projID = ModContent.ProjectileType<NightsGazeStar>();
                    int starDamage = SplitProjDamage;
                    float starKB = 5f;
                    Vector2 velocity = Projectile.velocity;

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projID, starDamage, starKB, Projectile.owner, 1f, 0f);
                    Main.projectile[p].penetrate = 1;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), Projectile.timeLeft);
            OnHitEffects();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), Projectile.timeLeft);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            int onHitCount = 6;
            int chanceOfStar = 2;
            float spread = 20f;
            int projectileDamage = SplitProjDamage;
            float kb = 5f;
            int sparkID = ModContent.ProjectileType<NightsGazeSpark>();
            int starID = ModContent.ProjectileType<NightsGazeStar>();
            for (int i = 0; i < onHitCount; i++)
            {
                int projID = Main.rand.NextBool(chanceOfStar) ? starID : sparkID;
                Vector2 velocity = Projectile.oldVelocity.RotateRandom(MathHelper.ToRadians(spread));
                float speed = Main.rand.NextFloat(1.5f, 2f);
                float moveDuration = Main.rand.Next(5, 15);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity * speed, projID, projectileDamage, kb, Projectile.owner, 0f, moveDuration);
            }

            SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * 0.6f}, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item68 with { Volume = SoundID.Item68.Volume * 0.2f}, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = SoundID.Item122.Volume * 0.4f}, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/NightsGazeGlow").Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    109,
                    111,
                    132
                });

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X / 3f, Projectile.velocity.Y / 3f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
