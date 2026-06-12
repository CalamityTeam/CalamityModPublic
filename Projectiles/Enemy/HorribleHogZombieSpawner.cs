using System.Collections.Generic;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogZombieSpawner : ModProjectile, ILocalizedModType
    {
        private static Asset<Texture2D> ZombieArmTexture;
        private static Asset<Texture2D> BackglowTexture;

        private static SoundStyle ZombieEmergeSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogZombieEmerge");

        public List<int> Zombies = new()
        {
            NPCID.Zombie,
            NPCID.FemaleZombie,
            NPCID.BaldZombie,
            NPCID.PincushionZombie,
            NPCID.SlimedZombie,
            NPCID.SwampZombie,
            NPCID.TwiggyZombie,
            NPCID.TorchZombie,
            NPCID.BloodZombie,
            NPCID.MaggotZombie,
            ModContent.NPCType<BucketZombie>(),
        };

        public static CalamityUtils.CurveSegment Rise => new(CalamityUtils.EasingType.ExpOut, 0f, 0f, 1.08f);

        public static CalamityUtils.CurveSegment Fallback => new(CalamityUtils.EasingType.SineOut, 0.08f, Rise.EndingHeight, -0.08f);

        public static CalamityUtils.CurveSegment Linger => new(CalamityUtils.EasingType.Linear, 0.20f, Fallback.EndingHeight, 0f);

        public static CalamityUtils.CurveSegment BounceUp => new(CalamityUtils.EasingType.PolyInOut, 0.82f, Linger.EndingHeight, 0.14f, 4);

        public static CalamityUtils.CurveSegment Descend => new(CalamityUtils.EasingType.PolyOut, 0.92f, BounceUp.EndingHeight, -1.48f, 3);

        public float ZombieArmRiseAnimation => CalamityUtils.PiecewiseAnimation(Timer / MaxTime, Rise, Fallback, Linger, BounceUp, Descend);

        public ref float Timer => ref Projectile.ai[0];

        public ref float MaxTime => ref Projectile.ai[1];

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                ZombieArmTexture = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.ZombieArm);
                BackglowTexture = ModContent.Request<Texture2D>("Terraria/Images/Extra_60");
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            Projectile.rotation = MathHelper.ToRadians(Main.rand.NextFloat(-8f, 8f));
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Timer >= MaxTime)
            {
                Projectile.Kill();
                return;
            }

            if (Main.rand.NextBool(6))
            {
                int dustAmt = Main.rand.Next(1, 2);
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 dustPosition = Projectile.Bottom + Main.rand.NextVector2Circular(12f, 0f);
                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, -2f));
                    int dustType = Main.rand.Next(2);
                    Dust.NewDustPerfect(dustPosition, dustType, dustVelocity);
                }
            }

            int dustCloudAmt = Main.rand.Next(1, 2);
            for (int i = 0; i < dustCloudAmt; i++)
            {
                Vector2 dustCloudPosition = Projectile.Bottom + Main.rand.NextVector2Circular(12f, 0f);
                float dustCloudScale = Main.rand.NextFloat(0.2f, 0.3f);
                Color dustCloudColor = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat());
                TimedSmokeParticle dustCloud = new(dustCloudPosition, Vector2.Zero, dustCloudColor, dustCloudColor, dustCloudScale, 1f, Main.rand.Next(15, 30));
                GeneralParticleHandler.SpawnParticle(dustCloud, true);
            }

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Raincoat Zombies get added when its raining.
                if (Main.raining)
                    Zombies.Add(NPCID.ZombieRaincoat);

                // Multiple Bucket Zombies and Blood Zombies get added in Revengeance and Death Mode to increase the chances of them spawning.
                if (CalamityWorld.revenge)
                {
                    for (int i = 0; i < 3; i++)
                        Zombies.Add(NPCID.BloodZombie);
                }

                if (CalamityWorld.death)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Zombies.Add(ModContent.NPCType<BucketZombie>());
                        Zombies.Add(NPCID.BloodZombie);
                    }
                }

                int zombieType = Utils.SelectRandom(Main.rand, [.. Zombies]);
                int zombie = NPC.NewNPC(Projectile.GetItemSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, zombieType);
                Main.npc[zombie].velocity = Vector2.UnitY * -4f;
            }

            int dustAmt = Main.rand.Next(12, 19);
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 dustPosition = Projectile.Bottom + Main.rand.NextVector2Circular(12f, 0f);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-6f, -4f);
                int dustType = Main.rand.Next(2);
                Dust.NewDust(dustPosition, 0, 0, dustType, dustVelocity.X, dustVelocity.Y);
            }

            int dustCloudAmt = Main.rand.Next(9, 13);
            for (int i = 0; i < dustCloudAmt; i++)
            {
                Vector2 dustCloudPosition = Projectile.Bottom + Main.rand.NextVector2Circular(12f, 0f);
                Vector2 dustCloudVelocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -4f));
                float dustCloudScale = Main.rand.NextFloat(1.2f, 1.6f);
                Color dustCloudColor = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat());
                SmallSmokeParticle dustCloud = new(dustCloudPosition, dustCloudVelocity, dustCloudColor, dustCloudColor, dustCloudScale, Main.rand.Next(120, 150));
                GeneralParticleHandler.SpawnParticle(dustCloud, true);
            }

            SoundEngine.PlaySound(ZombieEmergeSound, Projectile.Center);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float scaleX = MathHelper.Lerp(0f, 1f, ZombieArmRiseAnimation);
            Vector2 backglowScale = new(scaleX, 1f);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(BackglowTexture.Value, drawPosition, null, Color.Red with { A = 0 } * ZombieArmRiseAnimation, 0f, BackglowTexture.Size() * 0.5f, backglowScale, 0, 0f);

            float armScale = MathHelper.Lerp(0.2f, 1f, ZombieArmRiseAnimation);
            float armRotation = Projectile.rotation - MathHelper.PiOver4 + MathHelper.ToRadians(Main.rand.NextFloat(-2f, 2f));

            float height = MathHelper.Lerp(68f, 12f, ZombieArmRiseAnimation);
            Vector2 armDrawPostion = drawPosition + Vector2.UnitY * height;

            Main.spriteBatch.Draw(ZombieArmTexture.Value, armDrawPostion, null, lightColor, armRotation, ZombieArmTexture.Size() * new Vector2(0f, 1f), armScale, 0, 0f);
            return false;
        }
    }
}
