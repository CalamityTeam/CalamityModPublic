using System;
using System.Collections.Generic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Animosity : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ShootAndReloadSound = new("CalamityMod/Sounds/Item/WulfrumBlunderbussFireAndReload") { PitchVariance = 0.25f };
        // Very cool sound and it would be a shame for it to not be used elsewhere, would be even better if a new sound is made in the future, but for now this is good enough

        public float SniperDmgMult = 9f;
        public float SniperCritMult = Main.zenithWorld ? 7f : 1.35f;
        public float SniperVelocityMult = 2f;
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 18;
            Item.damage = 39;
            Item.DamageType = DamageClass.Ranged;
            Item.scale = 0.85f;
            Item.useTime = Item.useAnimation = 31;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 6.5f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 8;
        }

        public override void UpdateInventory(Player player)
        {
            //ITS MY REWORK SO I CAN PUT A REFERENCE: Shotgun Full of Hate, returns Animosity otherwise
            if (Main.zenithWorld)
                Item.SetNameOverride(this.GetLocalizedValue("GFBName"));
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string tooltip = Main.zenithWorld ? this.GetLocalizedValue("TooltipGFB") : this.GetLocalizedValue("TooltipNormal");
            list.FindAndReplace("[GFB]", Lang.SupportGlyphs(tooltip));
            //Distortion wind do whisper one truth...
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1 / 2f;

            return 1f;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.reuseDelay = 5;
            else
                Item.reuseDelay = 10;

            return base.CanUseItem(player);
        }

        #region Shooting
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //It should feel powerful but also not too much given feedback

            if (player.altFunctionUse == 2)
            {
                player.SetScreenshake(2f);
                SoundEngine.PlaySound(ShootAndReloadSound with { PitchVariance = 0.3f }, position);

                if (Main.zenithWorld) // Why only shotgun full of hate, why not Hexagun too? (See AnimosityBullet for more)
                {
                    SoundEngine.PlaySound(SoundID.Item9, position);
                    SoundEngine.PlaySound(SoundID.Item25, position);
                }
                //Shoot from muzzle
                Vector2 nuzzlePos = player.MountedCenter + velocity * 4f;

                //The dmg mult has to be applied here otherwise the left click gets it instead (for one shot), and the crit needs to be applied down here too cuz otherwise it never affects the weapon
                int p = Projectile.NewProjectile(source, nuzzlePos, velocity * SniperVelocityMult, ModContent.ProjectileType<AnimosityBullet>(), (int)(damage * SniperDmgMult), knockback, player.whoAmI);
                Main.projectile[p].CritChance = (int)(Main.projectile[p].CritChance * SniperCritMult); //To support crit mults with decimals
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item38 with { Volume = 0.8f, Pitch = 0.5f, PitchVariance = 0.3f }, position);
                //Shoot from muzzle
                Vector2 nuzzlePos = player.MountedCenter + velocity * 4f;

                // Fire a shotgun spread of bullets.
                for (int i = 0; i < 6; ++i)
                {
                    Vector2 randomVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(i * 2.5f));
                    Projectile shot = Projectile.NewProjectileDirect(source, nuzzlePos, randomVelocity, type, damage, knockback, player.whoAmI);
                    CalamityGlobalProjectile cgp = shot.Calamity();
                    cgp.brimstoneBullets = true; //add a brimstone trail to all bullets
                }
                for (int i = 0; i <= 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(nuzzlePos, DustID.SteampunkSteam, velocity.RotatedByRandom(MathHelper.ToRadians(7f)) * Main.rand.NextFloat(0.05f, 0.4f), 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                    dust.noGravity = true;
                    dust.alpha = 150;
                }

                // Fires other assorted things in GFB, because funni!
                if (Main.zenithWorld)
                {
                    // Packed full of skulls,
                    if (Main.rand.Next(4) < 3)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Vector2 skullVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(k * 2f));
                            Projectile skullHate = Projectile.NewProjectileDirect(source, nuzzlePos, skullVelocity, ProjectileID.BookOfSkullsSkull, damage / 4, knockback, player.whoAmI);
                            skullHate.DamageType = DamageClass.Ranged;
                            skullHate.extraUpdates += 1;
                            skullHate.penetrate = 1;
                        }
                    }
                    // nails,
                    if (Main.rand.Next(4) < 3)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            Vector2 nailVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(n * 2f));
                            Projectile nailHate = Projectile.NewProjectileDirect(source, nuzzlePos, nailVelocity, ProjectileID.NailFriendly, damage / 4, knockback, player.whoAmI);
                            nailHate.DamageType = DamageClass.Ranged;
                            nailHate.extraUpdates += 1;
                        }
                    }
                    // and poison.
                    if (Main.rand.Next(4) < 3)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            Vector2 poisonVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(p * 2f));
                            Projectile poisonHate = Projectile.NewProjectileDirect(source, nuzzlePos, poisonVelocity, ModContent.ProjectileType<AcidicSaxBubble>(), damage / 2, knockback, player.whoAmI, 0f, 0f, 1f);
                            poisonHate.DamageType = DamageClass.Ranged;
                            poisonHate.extraUpdates += 1;
                            poisonHate.penetrate = 1;
                        }
                    }
                }
            }
            //shell
            if (!Main.dedServ)
            {
                string goreType = Main.rand.NextBool() ? "EmptyAnimosityShell" : "EmptyAnimosityShell2";
                Gore.NewGore(source, position, velocity.RotatedBy(2f * -player.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type, 0.75f);
            }
            return false;
        }
        #endregion

        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-5, 6);


            //Sniper's horizontal recoil; can be a bit subtle but it is noticeable
            if (player.altFunctionUse == 2)
            {
                //Recoil:
                int anim = 0;
                for (int r = 0; r < Item.useAnimation; ++r)
                {
                    if (anim == 10 && r < Item.useAnimation / 2) //animates every 10 frames so that the player notices a recoil because this happens way too fast
                    {
                        itemPosition.X -= player.direction * 0.025f;
                        itemPosition.Y -= player.direction * 0.025f;
                        anim = 0;
                    }
                    else if (anim == 10 && r > Item.useAnimation / 2)
                    {
                        itemPosition.X += player.direction * 0.025f;
                        itemPosition.Y += player.direction * 0.025f;
                        anim = 0;
                    }
                    ++anim;
                }
            }

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        // Recoil + Not having the gun aim downwards
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (player.altFunctionUse == 2 ? -1f : -0.45f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation); //must be here otherwise it will vibrate


            //Reloads the gun 
            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }

        }
        #endregion
    }
}
