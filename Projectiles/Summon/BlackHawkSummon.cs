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
    public class BlackHawkSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public static Item FalseGun = null;
        public static Item BlackHawk = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.MiniTwins;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        // Defines an Item which is a hacked clone of a P90, edited to be summon class instead of ranged.
        // The false gun's damage is changed to the appropriate value every time a Black Hawk wants to fire a bullet.
        private static void DefineFalseGun(int baseDamage)
        {
            int p90ID = ModContent.ItemType<P90>();
            int BHRID = ModContent.ItemType<BlackHawkRemote>();
            FalseGun = new Item();
            BlackHawk = new Item();
            FalseGun.SetDefaults(p90ID, true);
            BlackHawk.SetDefaults(BHRID, true);
            FalseGun.damage = baseDamage;
            FalseGun.knockBack = BlackHawk.knockBack;
            FalseGun.shootSpeed = BlackHawk.shootSpeed;
            FalseGun.consumeAmmoOnFirstShotOnly = false;
            FalseGun.consumeAmmoOnLastShotOnly = false;

            FalseGun.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //Set namespaces
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();

            //On spawn effects
            if (Projectile.localAI[0] == 0f)
            {
                //Spawn dust
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 direction = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    direction = direction.RotatedBy((double)((dustIndex - (dustAmt / 2f - 1f)) * MathHelper.TwoPi / dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = direction - Projectile.Center;
                    int fire = Dust.NewDust(direction + dustVel, 0, 0, 258, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity = dustVel;
                }

                // Construct a fake item to use with vanilla code for the sake of firing bullets.
                if (FalseGun is null)
                    DefineFalseGun(Projectile.originalDamage);

                Projectile.localAI[0] += 1f;
            }

            //Update frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.frame = 0;
            }

            //Set up buff and timeLeft
            bool correctMinion = Projectile.type == ModContent.ProjectileType<BlackHawkSummon>();
            player.AddBuff(ModContent.BuffType<BlackHawkBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.blackhawk = false;
                }
                if (modPlayer.blackhawk)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //Anti sticky movement to prevent overlapping minions
            Projectile.MinionAntiClump();

            float maxDistance = 700f;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC && player.HasAmmo(FalseGun))
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!foundTarget && targetDist < (maxDistance + extraDist))
                    {
                        maxDistance = targetDist;
                        targetVec = npc.Center;
                        foundTarget = true;
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
                        if (!foundTarget && targetDist < (maxDistance + extraDist))
                        {
                            maxDistance = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //If too far, make the minion start returning to the player.
            float separationAnxietyDist = 1300f;
            if (foundTarget)
            {
                //Max travel distance increases if targeting something
                separationAnxietyDist = 2600f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            //If a target is found, move toward it
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 vecToTarget = targetVec - Projectile.Center;
                float targetDist = vecToTarget.Length();
                vecToTarget.Normalize();
                //If farther than 200 pixels, move toward it
                if (targetDist > 200f)
                {
                    float speedMult = 18f;
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
                //Otherwise, back it up slowly
                else
                {
                    float speedMult = -9f;
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
            }

            //If not targeting something, act passively
            else
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = Projectile.ai[0] == 1f;
                }
                //Move faster if actively returning to the player
                float speedMult = 12f;
                if (returningToPlayer)
                {
                    speedMult = 30f;
                }
                Vector2 vecToPlayer = player.Center - Projectile.Center + new Vector2(0f, -120f);
                float playerDist = vecToPlayer.Length();
                //Speed up if near the player
                if (playerDist < 200f && speedMult < 16f)
                {
                    speedMult = 16f;
                }
                //If close enough to the player, return to normal
                if (playerDist < 600f && returningToPlayer)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                //If abnormally far, teleport to the player
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                //Move toward player if more than 70 pixels away
                if (playerDist > 70f)
                {
                    vecToPlayer.Normalize();
                    vecToPlayer *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToPlayer) / 41f;
                }
                //Move if still
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }

            //Update rotation
            if (foundTarget)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            //Increment attack cooldown
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1]++;
            }
            //Set the minion to be ready for attack
            if (Projectile.ai[1] > 85f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            //Return if on attack cooldown, has no target, or returning to the player
            if (Projectile.ai[0] != 0f || !foundTarget || Projectile.ai[1] != 0f)
                return;

            //Shoot a bullet
            if (Main.myPlayer == Projectile.owner)
            {
                int projType = ModContent.ProjectileType<BlackHawkBullet>();
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                Projectile.ai[1] += 2f;

                // 50% chance to not consume ammo.
                bool dontConsumeAmmo = Main.rand.NextBool();
                int projIndex;

                // Vanilla function tricked into using a fake gun item with the appropriate base damage as the "firing item".
                player.PickAmmo(FalseGun, out int projID, out float shootSpeed, out int damage, out float kb, out _, dontConsumeAmmo);

                Vector2 velocity = Projectile.SafeDirectionTo(targetVec) * shootSpeed;

                // Musket Balls get converted to Black Hawk's special bullets
                if (projID == ProjectileID.Bullet)
                    projID = projType;

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

        //Does no contact damage
        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, spriteEffects);
            return false;
        }

        //Pretty glowmask
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/BlackHawkGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, spriteEffects);
        }
    }
}
