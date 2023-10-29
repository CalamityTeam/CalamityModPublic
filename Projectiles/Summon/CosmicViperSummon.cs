using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public static Item FalseGun = null;
        public static Item CosmicViper = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        // Defines an Item which is a hacked clone of a P90, edited to be summon class instead of ranged.
        // The false gun's damage is changed to the appropriate value every time a Cosmic Viper wants to fire a bullet.
        private static void DefineFalseGun(int baseDamage)
        {
            int p90ID = ModContent.ItemType<P90>();
            int CVEID = ModContent.ItemType<CosmicViperEngine>();
            FalseGun = new Item();
            CosmicViper = new Item();
            FalseGun.SetDefaults(p90ID, true);
            CosmicViper.SetDefaults(CVEID, true);
            FalseGun.damage = baseDamage;
            FalseGun.knockBack = CosmicViper.knockBack;
            FalseGun.shootSpeed = CosmicViper.shootSpeed;
            FalseGun.consumeAmmoOnFirstShotOnly = false;
            FalseGun.consumeAmmoOnLastShotOnly = false;

            FalseGun.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustType = Main.rand.NextBool(3) ? 56 : 242;
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dusty = Dust.NewDust(rotate + faceDirection, 0, 0, dustType, faceDirection.X * 1.75f, faceDirection.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity = faceDirection;
                }

                // Construct a fake item to use with vanilla code for the sake of firing bullets.
                if (FalseGun is null)
                    DefineFalseGun(Projectile.originalDamage);

                Projectile.localAI[0] += 1f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 0;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<CosmicViperSummon>();
            player.AddBuff(ModContent.BuffType<CosmicViperEngineBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cosmicViper = false;
                }
                if (modPlayer.cosmicViper)
                {
                    Projectile.timeLeft = 2;
                }
            }

            float colorScale = (float)Projectile.alpha / 255f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);

            Projectile.MinionAntiClump();

            float detectRange = 2200f;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC && player.HasAmmo(FalseGun))
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (targetDist < (detectRange + extraDist))
                    {
                        targetVec = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget && player.HasAmmo(FalseGun))
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (!foundTarget && targetDist < (detectRange + extraDist))
                        {
                            detectRange = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                            targetIndex = npcIndex;
                        }
                    }
                }
            }
            float returnDist = 1300f;
            if (foundTarget)
            {
                returnDist = 2600f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > returnDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 targetVector = targetVec - Projectile.Center;
                float targetDist = targetVector.Length();
                targetVector.Normalize();
                float speedMult = 30f; //12
                if (targetDist > 200f)
                {
                    targetVector *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 15f + targetVector) / 16f;
                }
                else
                {
                    targetVector *= -(speedMult / 2);
                    Projectile.velocity = (Projectile.velocity * 15f + targetVector) / 16f;
                }
            }
            else
            {
                float safeDist = 600f;
                bool returnToPlayer = false;
                if (!returnToPlayer)
                {
                    returnToPlayer = Projectile.ai[0] == 1f;
                }
                float velocityMult = 12f;
                if (returnToPlayer)
                {
                    velocityMult = 30f;
                }
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -120f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && velocityMult < 16f)
                {
                    velocityMult = 16f;
                }
                if (playerDist < safeDist && returnToPlayer)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= velocityMult;
                    Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            if (foundTarget)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                float xVelOffset = Projectile.velocity.X / 3f;
                float yVelOffset = Projectile.velocity.Y / 3f;
                int trail = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[trail];
                dust.position.X = Projectile.Center.X - xVelOffset;
                dust.position.Y = Projectile.Center.Y - yVelOffset;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1]++;
            }
            if (Projectile.ai[1] > 60f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (foundTarget && Projectile.ai[1] == 0f)
                {
                    //play cool sound
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                    Projectile.ai[1] += 2f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        // Fire a rocket every other time
                        bool shootRocket = ++Projectile.localAI[1] % 2f == 0f;
                        int projType = ModContent.ProjectileType<CosmicViperSplittingRocket>();
                        switch (Projectile.localAI[1] % 3f)
                        {
                            case 0f:
                                projType = ModContent.ProjectileType<CosmicViperSplittingRocket>();
                                break;
                            case 1f:
                                projType = ModContent.ProjectileType<CosmicViperHomingRocket>();
                                break;
                            case 2f:
                                projType = ModContent.ProjectileType<CosmicViperConcussionMissile>();
                                break;
                        }

                        // Rockets never consume ammo + 50% chance to not consume ammo.
                        bool dontConsumeAmmo = Main.rand.NextBool() || shootRocket;
                        int projIndex;

                        // Vanilla function tricked into using a fake gun item with the appropriate base damage as the "firing item".
                        player.PickAmmo(FalseGun, out int projID, out float shootSpeed, out int damage, out float kb, out _, dontConsumeAmmo);

                        Vector2 velocity = Projectile.SafeDirectionTo(targetVec) * shootSpeed;

                        // One in every 20 shots is a rocket which deals 1.5x total damage and extreme knockback.
                        if (shootRocket)
                        {
                            //add some inaccuracy
                            velocity.Y += Main.rand.NextFloat(-15f, 15f) * 0.05f;
                            velocity.X += Main.rand.NextFloat(-15f, 15f) * 0.05f;
                            projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, damage, kb, Projectile.owner);
                        }

                        // Fire the selected bullet, nothing special.
                        else
                            projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projID, damage, kb, Projectile.owner);

                        // Regardless of what was fired, force it to be a summon projectile so that summon accessories work.
                        if (projIndex.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[projIndex].DamageType = DamageClass.Summon;
                            Main.projectile[projIndex].minion = false;
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int y6 = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }


        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/CosmicViperGlow").Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int y6 = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, spriteEffects, 0);
        }
    }
}
