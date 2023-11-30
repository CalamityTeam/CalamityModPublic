using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnThrow2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        bool HasHitEnemy = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.0f;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = true; // We don't want people getting stuck in walls right
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 15;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //should be self explanatory
            width = 32;
            height = 32;
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            //Give iframes to the player
            if (player.immuneTime <= 30)
            { 
            player.immuneNoBlink = true;
            player.immuneTime = 30;
            }

            // Spawn homing flames that chase the HIT enemy only. This is also limited to one burst
            if (Main.myPlayer == Projectile.owner && !HasHitEnemy)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = Utils.NextVector2Circular(Main.rand, 7.2f, 7.2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                                             ModContent.ProjectileType<FinalDawnFireball>(),
                                             (int)(Projectile.damage * 0.2), Projectile.knockBack, Projectile.owner, 0f,
                                             target.whoAmI);
                }
                HasHitEnemy = true;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player is null || player.dead)
                Projectile.Kill();

            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(FinalDawn.UseSound, Projectile.position);
                Projectile.localAI[0] = 1;
            }

            // Kill any hooks from the projectile owner.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];

                if (!proj.active || proj.owner != player.whoAmI || proj.aiStyle != ProjAIStyleID.Hook)
                    continue;

                if (proj.aiStyle == ProjAIStyleID.Hook)
                    proj.Kill();
            }

            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation += 0.25f * Projectile.direction;
            player.Center = Projectile.Center;
            player.fullRotationOrigin = player.Center - player.position;
            player.fullRotation = Projectile.rotation;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.bodyFrame.Y = player.bodyFrame.Height;


            // This is to make sure the player doesn't get yeeted out of the world, which crashes the game pretty much all of the time
            bool worldEdge = Projectile.Center.X < 1000 || Projectile.Center.Y < 1000 || Projectile.Center.X > Main.maxTilesX * 16 - 1000 || Projectile.Center.Y > Main.maxTilesY * 16 - 1000;

            Projectile.ai[0]++;
            if(Projectile.ai[0] >= 60 || worldEdge)
            {
                Projectile.Kill();
            }

            int idx = Dust.NewDust(Projectile.position, Projectile.width , Projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 2.5f);
            Main.dust[idx].velocity = Projectile.velocity * -0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            float scytheRotation = player.fullRotation;

            Texture2D scytheTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowScytheTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/FinalDawnThrow2_Glow").Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;

            Vector2 origin = new Vector2(scytheTexture.Width / 2f + 40f * player.direction, num214 * 1.1f);

            Main.spriteBatch.Draw(scytheTexture,
                                  player.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, y6, scytheTexture.Width, num214)),
                                  Projectile.GetAlpha(lightColor),
                                  scytheRotation,
                                  origin,
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowScytheTexture,
                                  player.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, y6, scytheTexture.Width, num214)),
                                  Projectile.GetAlpha(Color.White),
                                  scytheRotation,
                                  origin,
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.fullRotation = 0;
        }
    }
}
