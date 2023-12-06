using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class NuclearFuryProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }

        public override void AI()
        {
	    if (Projectile.ai[1] == 1f)
		Projectile.penetrate = -1; // Gives infinite pierce to Valediction typhoons

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                int dustAmt = 6;
                for (int i = 0; i < dustAmt; ++i)
                {
                    Vector2 dustRotation = (Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width, (float)Projectile.height) / 2f).RotatedBy((double)(i - (dustAmt / 2 - 1)) * Math.PI / (double)dustAmt, new Vector2()) + Projectile.Center;
                    Vector2 randomRotation = ((Main.rand.NextFloat() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int nuclearDust = Dust.NewDust(dustRotation + randomRotation, 0, 0, 217, randomRotation.X * 2f, randomRotation.Y * 2f, 100, new Color(), 1.4f);
                    Dust dust = Main.dust[nuclearDust];
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity /= 4f;
                    dust.velocity -= Projectile.velocity;
                }
                Projectile.alpha -= 5;
                if (Projectile.alpha < 50)
                    Projectile.alpha = 50;
                Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.1f, 0.4f, 0.6f);
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.rotation += Projectile.velocity.Y * 0.1f;
            int npcID = -1;
            Vector2 targetVec = Projectile.Center;
            float maxDistance = 500f;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            if (Projectile.ai[0] == 0f && Projectile.localAI[0] == 0f)
            {
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(Projectile, false) && (Projectile.ai[0] == 0f || Projectile.ai[0] == (index + 1f)))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                        float npcDist = Vector2.Distance(npc.Center, targetVec);
                        if (npcDist < (maxDistance + extraDistance) && canHit)
                        {
                            maxDistance = npcDist;
                            targetVec = npc.Center;
                            npcID = index;
                        }
                    }
                }
                if (npcID >= 0)
                {
                    Projectile.ai[0] = npcID + 1f;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.localAI[0] == 0f && Projectile.ai[0] == 0f)
                Projectile.localAI[0] = 30f;
            bool isHoming = false;
            if (Projectile.ai[0] != 0f)
            {
                int index = (int)(Projectile.ai[0] - 1);
                if (Main.npc[index].active && !Main.npc[index].dontTakeDamage)
                {
                    if ((Math.Abs(Projectile.Center.X - Main.npc[index].Center.X) + Math.Abs(Projectile.Center.Y - Main.npc[index].Center.Y)) < 1000f)
                    {
                        isHoming = true;
                        targetVec = Main.npc[index].Center;
                    }
                }
                else
                {
                    Projectile.ai[0] = 0f;
                    isHoming = false;
                    Projectile.netUpdate = true;
                }
            }
            if (isHoming)
            {
                double homeVelocity = (double)(targetVec - Projectile.Center).ToRotation() - (double)Projectile.velocity.ToRotation();
                if (homeVelocity > Math.PI)
                    homeVelocity -= 2.0 * Math.PI;
                if (homeVelocity < -1.0 * Math.PI)
                    homeVelocity += 2.0 * Math.PI;
                Projectile.velocity = Projectile.velocity.RotatedBy(homeVelocity * 0.1, new Vector2());
            }
            float projSpeed = Projectile.velocity.Length();
            Projectile.velocity.Normalize();
            Projectile.velocity = Projectile.velocity * (projSpeed + 1f / 400f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 34, 0f, 0f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
