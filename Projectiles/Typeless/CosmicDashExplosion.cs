using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Projectiles.Typeless
{
    public class CosmicDashExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static readonly SoundStyle Impact = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak1") { Volume = 0.3f };
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Impact with { PitchVariance = 0.3f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f, PitchVariance = 0.3f }, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 35; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 181, new Vector2(4.5f, 4.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.9f), 0, default, Main.rand.NextFloat(1.5f, 2.8f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                dust.noGravity = true;
            }
            for (int j = 0; j < 14; j++)
            {
                Vector2 dustVel = new Vector2(6, 6).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.2f);

                Dust dust = Dust.NewDustPerfect(Projectile.Center + dustVel * 2, 272, dustVel, 0, default, 1f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);

                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + dustVel * 2, 226, dustVel, 0, default, 1f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
            }
        }
    }
}
