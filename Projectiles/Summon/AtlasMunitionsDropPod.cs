using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsDropPod : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public float TileCollisionYThreshold => Projectile.ai[0];

        public bool HasCollidedWithGround
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public const float Gravity = 0.45f;

        public const float MaxFallSpeed = 15.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drop Pod");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 14;
        }

        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 130;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y == 0f)
            {
                HasCollidedWithGround = true;
                Projectile.netUpdate = true;
            }

            Projectile.tileCollide = Projectile.Bottom.Y >= TileCollisionYThreshold;

            // Calculate frames.
            Projectile.frameCounter++;
            if (!HasCollidedWithGround) 
                Projectile.frame = Projectile.frameCounter / 6 % 5;
            else
            {
                Projectile.velocity.X = 0f;
                if (Projectile.frame < 5)
                    Projectile.frame = 5;
                if (Projectile.frameCounter % 8 == 7)
                {
                    Projectile.frame++;

                    // Release the upper part of the pod into the air and spawn the cannon.
                    if (Projectile.frame == 8)
                    {
                        SoundEngine.PlaySound(ThanatosHead.VentSound, Projectile.Top);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(28f, 10f), Vector2.Zero, ModContent.ProjectileType<AtlasMunitionsAutocannon>(), 0, 0f, Projectile.owner);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Top + new Vector2(28f, 72f), Vector2.Zero, ModContent.ProjectileType<AtlasMunitionsDropPodUpper>(), 0, 0f, Projectile.owner);
                        }
                    }
                }
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
            }

            // Fall downward.
            Projectile.velocity.Y += Gravity;
            if (Projectile.velocity.Y > MaxFallSpeed)
                Projectile.velocity.Y = MaxFallSpeed;
        }

        // As a means of obscuring contents when they spawn (such as ensuring that the minigun doesn't seem to pop into existence), this projectile draws above most other projectiles.
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsDropPodGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Color.White, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, 0, 0);
        }
    }
}
