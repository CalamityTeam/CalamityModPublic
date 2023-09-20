using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AbaddonCrit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        private static float ExplosionRadius = 300f;

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i <= 30; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 218 : ModContent.DustType<BrimstoneFlame>(), new Vector2(5, 5).RotatedByRandom(MathHelper.ToRadians(360)) * Main.rand.NextFloat(1.1f, 2.2f), 0, default, Main.rand.NextFloat(2.8f, 3.4f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cFace, player);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.DamageOverTime.BrimstoneFlames>(), 360);
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f, PitchVariance = 0.4f }, Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
        public override bool? CanDamage() => base.CanDamage();
        public override bool? CanCutTiles() => false;
    }
}
