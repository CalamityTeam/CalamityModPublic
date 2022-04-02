using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class DemonPortal : ModNPC
    {
        public ref float Time => ref npc.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Mysterious Portal");

        public override void SetDefaults()
        {
            npc.width = npc.height = 60;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 24660;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.netAlways = true;
            npc.aiStyle = -1;
            npc.Calamity().DoesNotGenerateRage = true;
            npc.Calamity().DoesNotDisappearInBossRush = true;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => npc.lifeMax = 24660;

        public override void AI()
        {
            npc.rotation += 0.18f;
            npc.Opacity = Utils.InverseLerp(0f, 30f, Time, true) * Utils.InverseLerp(420f, 390f, Time, true);
            npc.velocity = Vector2.Zero;
            npc.scale = npc.Opacity;

            if (Time == 300f)
            {
                if (Main.myPlayer == npc.target)
                    ReleaseThings();
                Main.PlaySound(SoundID.DD2_EtherianPortalOpen, npc.Center);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Time >= 420f)
            {
                npc.active = false;
                npc.netUpdate = true;
            }

            Time++;
        }

        public void ReleaseThings()
        {
            bool friendly = npc.life == 1;
            for (int i = 0; i < 6; i++)
            {
                int demon = Projectile.NewProjectile(npc.Center, Main.rand.NextVector2CircularEdge(4f, 4f), ModContent.ProjectileType<SuicideBomberDemon>(), 17000, 0f, npc.target);
                if (Main.projectile.IndexInRange(demon))
                {
                    Main.projectile[demon].ai[1] = Main.rand.Next(-40, 0);
                    Main.projectile[demon].friendly = friendly;
                    Main.projectile[demon].hostile = !friendly;
                    Main.projectile[demon].netUpdate = true;
                }
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<SuicideBomberDemon>())
                return false;

            return null;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool wouldDie = npc.life - (damage * (crit ? 2D : 1D)) <= 0;
            if (wouldDie)
            {
                npc.immortal = true;

                // Enrage demons if hit when already at 1 health.
                if (npc.life == 1)
                {
                    int demonType = ModContent.ProjectileType<SuicideBomberDemon>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].type != demonType || !Main.projectile[i].active || Main.projectile[i].hostile)
                            continue;

                        Main.projectile[i].hostile = true;
                        Main.projectile[i].friendly = false;
                        Main.projectile[i].netUpdate = true;
                    }
                }

                npc.life = 1;
                damage = 0D;
                return false;
            }
            return true;
        }

        public override bool CheckDead()
        {
            npc.life = 1;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.SetBlendState(BlendState.AlphaBlend);

            Texture2D portalTexture = Main.npcTexture[npc.type];
            Vector2 drawPosition = npc.Center - Main.screenPosition;
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;

            // Purple-black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f) * npc.Opacity * 1.8f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, npc.rotation, origin, npc.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(portalTexture, drawPosition, null, color, -npc.rotation, origin, npc.scale * 1.2f, SpriteEffects.None, 0f);

            // Purple portal.
            color = Color.Lerp(Color.Lerp(baseColor, Color.Purple, 0.55f), Color.Black, 0.66f) * npc.Opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, npc.rotation * 0.6f, origin, npc.scale * 1.2f, SpriteEffects.None, 0f);

            // Red portal.
            color = Color.Lerp(baseColor, Color.Red, 0.55f) * npc.Opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, npc.rotation * -0.6f, origin, npc.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
