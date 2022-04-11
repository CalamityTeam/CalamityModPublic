using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Other
{
    public class DemonPortal : ModNPC
    {
        public ref float Time => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Mysterious Portal");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 60;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 24660;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
            NPC.Calamity().DoesNotGenerateRage = true;
            NPC.Calamity().DoesNotDisappearInBossRush = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => NPC.lifeMax = 24660;

        public override void AI()
        {
            NPC.rotation += 0.18f;
            NPC.Opacity = Utils.GetLerpValue(0f, 30f, Time, true) * Utils.GetLerpValue(420f, 390f, Time, true);
            NPC.velocity = Vector2.Zero;
            NPC.scale = NPC.Opacity;

            if (Time == 300f)
            {
                if (Main.myPlayer == NPC.target)
                    ReleaseThings();
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Time >= 420f)
            {
                NPC.active = false;
                NPC.netUpdate = true;
            }

            Time++;
        }

        public void ReleaseThings()
        {
            bool friendly = NPC.life == 1;
            for (int i = 0; i < 6; i++)
            {
                int demon = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Main.rand.NextVector2CircularEdge(4f, 4f), ModContent.ProjectileType<SuicideBomberDemon>(), 17000, 0f, NPC.target);
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
            bool wouldDie = NPC.life - (damage * (crit ? 2D : 1D)) <= 0;
            if (wouldDie)
            {
                NPC.immortal = true;

                // Enrage demons if hit when already at 1 health.
                if (NPC.life == 1)
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

                NPC.life = 1;
                damage = 0D;
                return false;
            }
            return true;
        }

        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.SetBlendState(BlendState.AlphaBlend);

            Texture2D portalTexture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;

            // Purple-black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f) * NPC.Opacity * 1.8f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, NPC.rotation, origin, NPC.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(portalTexture, drawPosition, null, color, -NPC.rotation, origin, NPC.scale * 1.2f, SpriteEffects.None, 0f);

            // Purple portal.
            color = Color.Lerp(Color.Lerp(baseColor, Color.Purple, 0.55f), Color.Black, 0.66f) * NPC.Opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, NPC.rotation * 0.6f, origin, NPC.scale * 1.2f, SpriteEffects.None, 0f);

            // Red portal.
            color = Color.Lerp(baseColor, Color.Red, 0.55f) * NPC.Opacity * 1.6f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, NPC.rotation * -0.6f, origin, NPC.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
