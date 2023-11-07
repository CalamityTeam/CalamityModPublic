using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantStar2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RadiantStar";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 270 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (Projectile.ai[0] == 1f)
            {
                float projX = Projectile.Center.X;
                float projY = Projectile.Center.Y;
                float homeRange = Projectile.Calamity().stealthStrike ? 1800f : 600f;
                float homingSpeed = 0.25f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float npcCenterY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float targetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                        if (targetDist < homeRange)
                        {
                            if (Main.npc[i].position.X < projX)
                            {
                                Main.npc[i].velocity.X += homingSpeed;
                            }
                            else
                            {
                                Main.npc[i].velocity.X -= homingSpeed;
                            }
                            if (Main.npc[i].position.Y < projY)
                            {
                                Main.npc[i].velocity.Y += homingSpeed;
                            }
                            else
                            {
                                Main.npc[i].velocity.Y -= homingSpeed;
                            }
                        }
                    }
                }
            }
            else if (Projectile.timeLeft < 270)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, Projectile.Calamity().stealthStrike ? 900f : 450f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
