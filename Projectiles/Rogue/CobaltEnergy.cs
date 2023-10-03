using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class CobaltEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int index = 0; index < 2; ++index)
            {
                int ruby = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 88, Projectile.velocity.X, Projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            if (!hasHitEnemy && Projectile.timeLeft < 575)
            {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
            }
            else if (hasHitEnemy)
            {
                if (targetNPC >= 0)
                {
                    Vector2 newVelocity = Main.npc[targetNPC].Center - Projectile.Center;
                    newVelocity.Normalize();
                    newVelocity *= 15f;
                    Projectile.velocity = newVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float minDist = 999f;
            int index = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                bool hasHitNPC = false;
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == i)
                    {
                        hasHitNPC = true;
                    }
                }

                NPC npc = Main.npc[i];
                if (npc == target)
                {
                    previousNPCs.Add(i);
                }
                if (npc.CanBeChasedBy(Projectile, false) && npc != target && !hasHitNPC)
                {
                    float dist = (Projectile.Center - npc.Center).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
            }

            Vector2 velocityNew;
            if (minDist < 999f)
            {
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - Projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                Projectile.velocity = velocityNew;
            }
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
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int ruby = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 88, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 50, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.scale *= 1.25f;
                dust.velocity *= 0.5f;
            }
        }
    }
}
