using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TinyCrystal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 90;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 75 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            int dustType = Projectile.ai[0] == 0f ? 56 : 73;
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int dustPosModifier = 4;
                int crystalDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPosModifier, Projectile.position.Y + (float)dustPosModifier), Projectile.width - dustPosModifier * 2, Projectile.height - dustPosModifier * 2, dustType, 0f, 0f, 100, default, 1.2f);
                Main.dust[crystalDust].noGravity = true;
                Main.dust[crystalDust].velocity *= 0.1f;
                Main.dust[crystalDust].velocity += Projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[crystalDust];
                expr_47FA_cp_0.position.X -= shortXVel;
                Dust expr_4815_cp_0 = Main.dust[crystalDust];
                expr_4815_cp_0.position.Y -= shortYVel;
            }

            if (Main.rand.NextBool(10))
            {
                int dustPosModifierAgain = 4;
                int extraCrystalDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPosModifierAgain, Projectile.position.Y + (float)dustPosModifierAgain), Projectile.width - dustPosModifierAgain * 2, Projectile.height - dustPosModifierAgain * 2, dustType, 0f, 0f, 100, default, 0.6f);
                Main.dust[extraCrystalDust].velocity *= 0.25f;
                Main.dust[extraCrystalDust].velocity += Projectile.velocity * 0.5f;
            }

            if (Projectile.timeLeft < 75)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 9f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            int dustType = Projectile.ai[0] == 0f ? 56 : 73;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int buffType = Projectile.ai[0] == 0f ? BuffID.Frostburn2 : BuffID.OnFire;
            target.AddBuff(buffType, 90);
        }
    }
}
