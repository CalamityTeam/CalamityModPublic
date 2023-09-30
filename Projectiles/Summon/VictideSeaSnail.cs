using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Summon
{
    public class VictideSeaSnail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float DustTimer => ref Projectile.localAI[0];
        public ref float FireCooldown => ref Projectile.localAI[1];
        public ref float PeekingOut => ref Projectile.ai[0];
        public ref float PlayerStandStillTimer => ref Projectile.ai[1];

        public const int frameTime = 6;
        public const int timeToStandStillBeforePeekOut = 200;

        public bool CanComePeekOut => !Main.player[Projectile.owner].mount.Active && PlayerStandStillTimer >= timeToStandStillBeforePeekOut || Projectile.frame > 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.victideSummoner)
            {
                for (int d = 0; d < 45; d++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 179, 0f, 0f, 0, default, 1f);
                    dust.velocity *= 2f;
                    dust.scale *= 1.15f;
                }
                Projectile.active = false;
                return;
            }

            if (player.dead)
                modPlayer.victideSnail = false;

            if (modPlayer.victideSnail)
                Projectile.timeLeft = 2;

            //Create a burst of dust as it gets created
            DustTimer++;
            if (DustTimer <= 3)
            {
                int dustAmount = Main.rand.Next(40, 50);
                for (int d = 0; d < dustAmount; d++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 179, 0f, 0f, 0, default, 1f);
                    dust.velocity *= 2f;
                    dust.scale *= 1.15f;
                }
            }

            if (player.velocity.Length() == 0)
            {
                PlayerStandStillTimer++;
                //pop out the shell
                if (PlayerStandStillTimer == timeToStandStillBeforePeekOut )
                {
                    DustTimer = 0;
                    float direction = Main.rand.NextBool() ? 1f : -1f;
                    Projectile.velocity = new Vector2(Main.rand.NextFloat(4f, 10f) * direction, Main.rand.NextFloat(-6f, 0f));
                }

                Projectile.velocity *= 0.98f;
                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 14f);
                Projectile.tileCollide = true;

                if (CanComePeekOut)
                    Projectile.rotation = Projectile.rotation.AngleTowards(0, MathHelper.PiOver4 * 0.14f);

                if (PeekingOut > 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > frameTime)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame = Math.Min(6, Projectile.frame + 1);
                    }
                }
            }

            //Go back, in the shell
            if (player.velocity.Length() > 0 && CanComePeekOut)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > frameTime)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame--;
                }

                if (Projectile.frame == 0)
                {
                    PlayerStandStillTimer = 0f;
                }
            }

            if (!CanComePeekOut)
            {
                PeekingOut = 0f;

                if (player.velocity.Length() > 0)
                    PlayerStandStillTimer = 0f;

                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                Vector2 desiredPosition = player.Center + Vector2.UnitY * (player.gfxOffY - 60f) * player.gravDir;
                //Round the result so it doesn't jitter.
                desiredPosition = new Vector2((int)desiredPosition.X, (int)desiredPosition.Y);

                Projectile.rotation += MathHelper.PiOver4 * 0.2f;

                Projectile.Center = Projectile.Center.MoveTowards(desiredPosition, 7f + player.velocity.Length());
            }

            //Fire a projectile
            if (Projectile.owner == Main.myPlayer)
            {
                if (FireCooldown > 0f)
                {
                    FireCooldown --;
                    return;
                }

                bool foundTarget = false;
                float maxDist = 300f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        if (Vector2.Distance(Projectile.Center, npc.Center) < maxDist && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            foundTarget = true;
                            break;
                        }
                    }
                }

                if (foundTarget)
                {
                    int projAmt = Main.rand.Next(3, 7);
                    for (int u = 0; u < projAmt; u++)
                    {
                        Vector2 source = new Vector2(Projectile.Center.X - 4f, Projectile.Center.Y);
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, ModContent.ProjectileType<UrchinSpike>(), Projectile.damage, 1f, Projectile.owner);
                    }

                    SoundEngine.PlaySound(SoundID.Item42, Projectile.position);
                    FireCooldown = 60f;
                }
            }

            if ((Projectile.Center - player.Center).Length() > 400)
            {
                DustTimer = 0;
                Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f) * player.gravDir;
                PlayerStandStillTimer = 0f;
                Projectile.frame = 0;
            }
        }

        public override bool? CanDamage() => false;

        public override void OnKill(int timeLeft)
        {
            for (int d = 0; d < 45; d++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 179, 0f, 0f, 0, default, 1f);
                dust.velocity *= 2f;
                dust.scale *= 1.15f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Touches the ground
            if (oldVelocity.Y >= 0)
            {
                PeekingOut = 1f;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(0, 36 * Projectile.frame, 38, 34);
            Vector2 origin = !CanComePeekOut ? new Vector2(15, 23) : frame.Size() / 2f;

            //Look at the player
            SpriteEffects flipperoni = Math.Sign(Projectile.position.X - owner.position.X) > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, flipperoni, 0);

            return false;
        }
    }
}
