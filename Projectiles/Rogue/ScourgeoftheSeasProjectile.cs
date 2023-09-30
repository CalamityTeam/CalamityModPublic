using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeoftheSeasProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ScourgeoftheSeas";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.StickProjectile;
            AIType = ProjectileID.BoneJavelin;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                if (Projectile.Calamity().stealthStrike) //stealth strike attack
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 116, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                }
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            Projectile.velocity.X *= 1.015f;
            Projectile.velocity.Y *= 1.015f;
            Projectile.velocity.X = Math.Min(16f, Projectile.velocity.X);
            Projectile.velocity.Y = Math.Min(16f, Projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
            if (Projectile.Calamity().stealthStrike) //stealth strike attack
            {
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 180);
            if (Projectile.Calamity().stealthStrike) //stealth strike attack
            {
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int dustIndex = 0; dustIndex < 8; dustIndex++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[dusty].velocity *= 1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int cloudNumber = Main.rand.Next(3, 5);
                for (int cloudIndex = 0; cloudIndex < cloudNumber; cloudIndex++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<ScourgeVenomCloud>(), (int)(Projectile.damage * 0.3), 1f, Projectile.owner, 0f, Projectile.Calamity().stealthStrike ? 1f : 0f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
