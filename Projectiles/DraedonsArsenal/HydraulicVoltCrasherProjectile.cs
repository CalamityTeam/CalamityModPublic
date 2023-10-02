using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class HydraulicVoltCrasherProjectile : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<HydraulicVoltCrasher>();
        public Player Owner => Main.player[Projectile.owner];
        private int chargeCooldown = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.timeLeft = 60;
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 4f == 3f)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            // Decrement charge cooldown
            if (chargeCooldown > 0)
                chargeCooldown = 0;
            // Play idle sounds every so often.
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.position);
                Projectile.soundDelay = 30;
            }
            Vector2 center = Owner.RotatedRelativePoint(Owner.MountedCenter);

            if (Main.myPlayer == Owner.whoAmI)
            {
                // Get the projectile owner's held item. If it's not a modded item, stop now to prevent weird errors.
                Item heldItem = Owner.ActiveItem();
                if (heldItem.type < ItemID.Count)
                {
                    Projectile.Kill();
                    return;
                }

                // Update the damage of the holdout projectile constantly so that it decreases as charge decreases, even while in use.
                Projectile.damage = Owner.GetWeaponDamage(heldItem);

                // Check if the player's held item still has sufficient charge. If so, and they're still using it, take a tiny bit of charge from it.
                CalamityGlobalItem modItem = heldItem.Calamity();
                if (Owner.channel && modItem.Charge >= HydraulicVoltCrasher.HoldoutChargeUse)
                {
                    modItem.Charge -= HydraulicVoltCrasher.HoldoutChargeUse;

                    float speed = Owner.inventory[Owner.selectedItem].shootSpeed * Projectile.scale;
                    Vector2 toPointTo = Main.MouseWorld;
                    if (Owner.gravDir == -1f)
                    {
                        toPointTo.Y = Main.screenHeight - toPointTo.Y;
                    }
                    toPointTo = Vector2.Normalize(toPointTo - center) * speed;
                    if (toPointTo != Projectile.velocity)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = toPointTo;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Owner.ChangeDir((Projectile.velocity.X > 0).ToDirectionInt());
            Owner.heldProj = Projectile.whoAmI;

            Owner.itemAnimation = 2;
            Owner.itemTime = 2; // Note: Owner.SetDummyItemTime(frame) exists in 1.4 which accomplishes this task

            Owner.itemRotation = (Projectile.velocity * Owner.direction).ToRotation();
            Projectile.direction = Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1).ToInt() * MathHelper.Pi;

            if (Main.rand.NextBool(5))
            {
                Vector2 spawnPosition = Projectile.velocity;
                spawnPosition.Normalize();
                spawnPosition *= Projectile.Size;
                spawnPosition += Projectile.Center;
                Dust dust = Dust.NewDustPerfect(spawnPosition, 226);
                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 3.6f);
                dust.velocity += Owner.velocity * 0.4f;
            }
            Projectile.position = center - Projectile.Size * 0.5f;
            Projectile.position -= Projectile.velocity.ToRotation().ToRotationVector2() * 8f;
            Projectile.velocity.X *= Main.rand.NextFloat(0.97f, 1.03f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBoltSound, target.Center);
            if (chargeCooldown > 0)
                return;
            chargeCooldown = 60;
            TryToSuperchargeNPC(target);
            int extraZaps = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (i != target.whoAmI &&
                    Main.npc[i].CanBeChasedBy(Projectile, false) &&
                    Main.npc[i].Distance(target.Center) < 240f &&
                    extraZaps < 3)
                {
                        
                    if (TryToSuperchargeNPC(Main.npc[i]))
                    {
                        for (float increment = 0f; increment <= 1f; increment += 0.05f)
                        {
                            Vector2 spawnPosition = Vector2.Lerp(target.Center, Main.npc[i].Center, increment);
                            Dust dust = Dust.NewDustPerfect(spawnPosition, 226);
                            dust.velocity = Vector2.Zero;
                            dust.scale = 1.6f;
                            dust.noGravity = true;
                        }
                        extraZaps++;
                    }
                }
            }
        }

        public bool TryToSuperchargeNPC(NPC npc)
        {
            int attackType = ModContent.ProjectileType<VoltageStream>();

            // No more than 3 enemies with streams
            if (Owner.ownedProjectileCounts[attackType] > 3)
                return false;
            
            // Prevent supercharging an enemy twice.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == attackType &&
                    Main.projectile[i].ai[1] == i)
                {
                    return false;
                }
            }
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center,
                                           Vector2.Zero,
                                           attackType,
                                           Projectile.damage,
                                           0f,
                                           Projectile.owner,
                                           0f,
                                           npc.whoAmI);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), lightColor, Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
