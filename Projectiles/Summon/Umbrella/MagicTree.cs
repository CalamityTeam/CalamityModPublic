using CalamityMod.Gores.Trees;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicTree : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Summon/Umbrella/TreeForest";
        private Tree TreeType = Tree.Forest;
        private enum Tree
        {
            Astral,
			Corruption,
			Crimson,
			Forest,
			Hallow,
			Jungle,
			Ocean,
			Snow,
			SulphurousSea
        }
        public static readonly SoundStyle TreeCrashSound = new("CalamityMod/Sounds/Custom/TreeFalling");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            Projectile.rotation += 0.125f * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
			if (Projectile.ai[1] == 0f)
			{
				Array values = Enum.GetValues(typeof(Tree));
				Random random = new Random();
				TreeType = (Tree)values.GetValue(random.Next(values.Length));
				Projectile.ai[1]++;
			}
        }

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(TreeCrashSound with { Volume = 0.5f }, Projectile.Center);

            int creatureAmt = 3;
            for (int t = 0; t < creatureAmt; t++)
            {
				int projType = Utils.SelectRandom(Main.rand, new int[]
				{
					ModContent.ProjectileType<MagicBunny>(),
					ModContent.ProjectileType<MagicBird>()
				});
				Vector2 velocity = Vector2.Zero;
				if (Main.projectile.IndexInRange((int)Projectile.ai[0]))
				{
					velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(Projectile.Center, Main.npc[(int)Projectile.ai[0]].Center, 0.28f, 12f);
					velocity.X += Main.rand.NextFloat(-3f, 3f);
				}
				else
				{
					velocity.X = Main.rand.NextFloat(-10f, 10f);
					velocity.Y = Main.rand.NextFloat(-15f, -8f);
				}
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, (int)(Projectile.damage * 0.05f), 0f, Projectile.owner);
            }

			int treeGore = GoreID.TreeLeaf_Normal;
			string TreeTop = "TreeForestTop";
			string TreeBottom = "TreeForestBottom";
            switch (TreeType)
            {
                case Tree.Astral:
                    treeGore = ModContent.GoreType<AstralLeaf>();
					TreeTop = "TreeAstralTop";
					TreeBottom = "TreeAstralBottom";
                    break;
                case Tree.Corruption:
                    treeGore = GoreID.TreeLeaf_Corruption;
					TreeTop = "TreeCorruptionTop";
					TreeBottom = "TreeCorruptionBottom";
                    break;
                case Tree.Crimson:
                    treeGore = GoreID.TreeLeaf_Crimson;
					TreeTop = "TreeCrimsonTop";
					TreeBottom = "TreeCrimsonBottom";
                    break;
                case Tree.Forest:
                    treeGore = GoreID.TreeLeaf_Normal;
					TreeTop = "TreeForestTop";
					TreeBottom = "TreeForestBottom";
                    break;
                case Tree.Hallow:
                    treeGore = GoreID.TreeLeaf_Hallow;
					TreeTop = "TreeHallowTop";
					TreeBottom = "TreeHallowBottom";
                    break;
                case Tree.Jungle:
                    treeGore = GoreID.TreeLeaf_Jungle;
					TreeTop = "TreeJungleTop";
					TreeBottom = "TreeJungleBottom";
                    break;
                case Tree.Ocean:
                    treeGore = GoreID.TreeLeaf_Palm;
					TreeTop = "TreeOceanTop";
					TreeBottom = "TreeOceanBottom";
                    break;
                case Tree.Snow:
                    treeGore = GoreID.TreeLeaf_Boreal;
					TreeTop = "TreeSnowTop";
					TreeBottom = "TreeSnowBottom";
                    break;
                case Tree.SulphurousSea:
                    treeGore = ModContent.GoreType<SulphurLeaf>();
					TreeTop = "TreeSulphurousSeaTop";
					TreeBottom = "TreeSulphurousSeaBottom";
                    break;
                default:
                    break;
            }

			if (Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 20; i++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					Vector2 spawnSource = Projectile.Center;
					spawnSource.X += Main.rand.Next(-Projectile.width / 2, Projectile.width / 2 + 1);
					spawnSource.Y += Main.rand.Next(-Projectile.height / 2, Projectile.height / 2 + 1);
					int idx = Gore.NewGore(Projectile.GetSource_FromThis(), spawnSource, velocity, treeGore, 1f);
					Main.gore[idx].velocity.X *= Main.rand.NextFloat(0.5f, 5f);
					Main.gore[idx].velocity.Y *= Main.rand.NextFloat(0.5f, 5f);
					Main.gore[idx].velocity.X += Main.rand.NextFloat(-5f, 5f);
					Main.gore[idx].velocity.Y += Main.rand.NextFloat(-5f, 5f);
					Main.gore[idx].scale *= Main.rand.NextFloat(0.8f, 1.2f);
				}
				Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, CalamityUtils.RandomVelocity(100f, 70f, 100f) * Main.rand.NextFloat(), Mod.Find<ModGore>(TreeTop).Type, Projectile.scale);
				Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, CalamityUtils.RandomVelocity(100f, 70f, 100f) * Main.rand.NextFloat(), Mod.Find<ModGore>(TreeBottom).Type, Projectile.scale);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
			if (Projectile.ai[1] == 0f)
				return false;

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            switch (TreeType)
            {
                case Tree.Astral:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeAstral").Value;
                    break;
                case Tree.Corruption:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeCorruption").Value;
                    break;
                case Tree.Crimson:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeCrimson").Value;
                    break;
                case Tree.Forest:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeForest").Value;
                    break;
                case Tree.Hallow:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeHallow").Value;
                    break;
                case Tree.Jungle:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeJungle").Value;
                    break;
                case Tree.Ocean:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeOcean").Value;
                    break;
                case Tree.Snow:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeSnow").Value;
                    break;
                case Tree.SulphurousSea:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/TreeSulphurousSea").Value;
                    break;
                default:
                    break;
            }

			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw the tree.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override bool? CanDamage() => Projectile.ai[1] == 0f ? false : (bool?)null;
    }
}
