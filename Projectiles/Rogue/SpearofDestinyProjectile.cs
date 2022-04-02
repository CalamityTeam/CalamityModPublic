using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpearofDestinyProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SpearofDestiny";

        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (!initialized)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    projectile.penetrate++;
                    projectile.tileCollide = false;
                }
                initialized = true;
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Vector2 center = projectile.Center;
            float maxDistance = 300f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);
                    bool canHit = projectile.Calamity().stealthStrike || Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                    if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                projectile.extraUpdates = 2;
                Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                projectile.velocity = (projectile.velocity * 20f + moveDirection * 12f) / (21f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.Calamity().stealthStrike)
            {
                Texture2D tex = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SpearofDestinyGlow");
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            }
        }
    }
}
