using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class EnchantedAxe : ModProjectile
    {
        private bool recall = false;
        private bool summonAxe = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Axe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft < 585)
                {
                    recall = true;
                }
                //Periodically emit 
            }
            else
            {
                if (projectile.timeLeft < 590)
                {
                    recall = true;
                }
            }

            projectile.rotation += 0.4f * projectile.direction;

            if (recall)
            {
                Vector2 posDiff = Main.player[projectile.owner].position - projectile.position;
                if (posDiff.Length() > 30f)
                {
                    posDiff.Normalize();
                    projectile.velocity = posDiff * 30f;
                }
                else
                {
                    projectile.timeLeft = 0;
                    Kill(projectile.timeLeft);
                }

                if (summonAxe)
                {
                    float minDist = 999f;
                    int index = 0;
                    // Get the closest enemy to the axe
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.friendly && !npc.townNPC && npc.active)
                        {
                            float dist = (projectile.Center - npc.Center).Length();
                            if (dist < minDist)
                            {
                                minDist = dist;
                                index = i;
                            }
                        }
                    }
                    Vector2 newAxeVelocity;
                    if (minDist < 999f)
                    {
                        newAxeVelocity = Main.npc[index].Center - projectile.Center;
                    }
                    else
                    {
                        newAxeVelocity = -projectile.velocity;
                    }
                    newAxeVelocity.Normalize();
                    newAxeVelocity *= 30f;
                    int p = Projectile.NewProjectile(projectile.position, newAxeVelocity, ModContent.ProjectileType<EnchantedAxe2>(), 15, 2, projectile.owner, 0, 0);
                    Main.projectile[p].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
                    summonAxe = false;
                }
            }
            else
            {
                if (projectile.timeLeft % 5 == 1 && projectile.Calamity().stealthStrike)
                {
                    float axeSpeed = 15f;
                    Projectile.NewProjectile(projectile.position, new Vector2(1f, 0f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, new Vector2(0f, 1f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, new Vector2(-1f, 0f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, new Vector2(0f, -1f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, Vector2.Normalize(new Vector2(1f, 1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, Vector2.Normalize(new Vector2(1f, -1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, Vector2.Normalize(new Vector2(-1f, -1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                    Projectile.NewProjectile(projectile.position, Vector2.Normalize(new Vector2(-1f, 1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), 20, 2, projectile.owner, 0, 0);
                }
            }

            if (projectile.position == Main.player[projectile.owner].position)
            {
                Kill(projectile.timeLeft);
            }
            return;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 3);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (recall)
            {
                return false;
            }
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            recall = true;
            projectile.tileCollide = false;
            return false;
        }
    }
}
