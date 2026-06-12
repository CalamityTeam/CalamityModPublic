using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ProfanedPartisanSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void AI()
        {
            if (Projectile.originalDamage == 0)
            {
                Projectile.originalDamage = ProfanedPartisan.SpearBaseDamage;
                Projectile.ContinuouslyUpdateDamageStats = true;
            }

            float timeGateValue = 30f;
            if (Projectile.ai[0] <= 0f) //Copied from HolySpear.cs
            {
                Projectile.ai[1] += 1f;

                float slowGateValue = 25f;
                float fastGateValue = 10f;
                float minVelocity = 2f;
                float maxVelocity = minVelocity * 5f;
                float extremeVelocity = maxVelocity * 2f;
                float deceleration = 0.9f;
                float acceleration = 2f;

                if (Projectile.localAI[1] >= timeGateValue)
                {
                    if (Projectile.velocity.Length() < extremeVelocity)
                        Projectile.velocity *= acceleration;
                }
                else
                {
                    if (Projectile.ai[1] <= slowGateValue)
                    {
                        if (Projectile.velocity.Length() > minVelocity)
                            Projectile.velocity *= deceleration;
                    }
                    else if (Projectile.ai[1] < slowGateValue + fastGateValue)
                    {
                        if (Projectile.velocity.Length() < maxVelocity)
                            Projectile.velocity *= acceleration;
                    }
                    else
                        Projectile.ai[1] = 0f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            CalamityUtils.DrawAfterimagesCentered(Projectile, 2, Color.White);
            return false;
        }
    }
}
