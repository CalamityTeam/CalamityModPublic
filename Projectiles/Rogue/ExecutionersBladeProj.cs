using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExecutionersBladeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ExecutionersBlade";

        private void handleStealth(Vector2 position)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(position.X + (-600 + i * 60), position.Y - 800), new Vector2(0f, 2.5f), ModContent.ProjectileType<ExecutionersBladeStealthProj>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 240;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0.65f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 12f, 20f);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(32f, 32f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ExecutionersBladeGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (Projectile.Calamity().stealthStrike)
            {
                handleStealth(target.Center);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (Projectile.Calamity().stealthStrike)
            {
                handleStealth(target.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(128);
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 5; j++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 3f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
