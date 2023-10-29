using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class RegulusEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            int constant = 14;
            int coolDust;
            Projectile.ai[0] += 1;
            if (Projectile.ai[0] % 2 == 0)
            {
                if (Projectile.ai[0] % 4 == 0)
                {
                    coolDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - constant * 2, Projectile.height - constant * 2, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                }
                else
                {
                    coolDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - constant * 2, Projectile.height - constant * 2, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                }
                Main.dust[coolDust].noGravity = true;
                Main.dust[coolDust].velocity *= 0.1f;
                Main.dust[coolDust].velocity += Projectile.velocity * 0.5f;
            }
            if (Projectile.ai[0] < 90)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y *= 0.98f;
            }
            else
            {
                Projectile.extraUpdates = 1;

                Vector2 center = Projectile.Center;
                float maxDistance = 500f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 12f) / (21f);
                }
                else
                    Projectile.Kill();
            }

            Projectile.rotation += 0.25f;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 90 ? null : false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 24;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int otherConstant = 36;
            for (int j = 0; j < otherConstant; j++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(j - (otherConstant / 2 - 1)) * 6.28318548f / (float)otherConstant), default) + Projectile.Center;
                Vector2 facingDirection = rotate - Projectile.Center;
                int dusty = Dust.NewDust(rotate + facingDirection, 0, 0, ModContent.DustType<AstralOrange>(), facingDirection.X * 0.5f, facingDirection.Y * 0.5f, 100, default, 0.75f);
                Main.dust[dusty].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60);
    }
}
