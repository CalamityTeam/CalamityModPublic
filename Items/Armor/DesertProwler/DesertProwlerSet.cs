using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Cooldowns;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod.Particles;
using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Audio;
using Terraria.DataStructures;
using CalamityMod.Dusts;
using ReLogic.Utilities;
using Terraria.Localization;

namespace CalamityMod.Items.Armor.DesertProwler
{
    [AutoloadEquip(EquipType.Head)]
    public class DesertProwlerHat : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public static readonly SoundStyle SmokeBombSound = new("CalamityMod/Sounds/Custom/AbilitySounds/DesertProwlerSmokeBomb");
        public static readonly SoundStyle SmokeBombEndSound = new("CalamityMod/Sounds/Custom/AbilitySounds/DesertProwlerSmokeBombEnd");
        public static readonly SoundStyle CDResetSound = new("CalamityMod/Sounds/Custom/AbilitySounds/DesertProwlerCDReset");

        public static int FlatDamageBonus = 1;
        public static int SmokeCooldown = 25 * 60;
        public static int SmokeDuration = 5 * 60;
        public static int LightsOutReset = (int)(1.5f * 60);
        public static int FreeCrit = 200;
        public static int BonusDamageCap = 100;

        public static bool ShroudedInSmoke(Player player, out CooldownInstance cd)
        {
            cd = null;
            bool hasSmokebombCD = player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out cd);
            return (hasSmokebombCD && cd.timeLeft > SmokeCooldown);
        }

        public override void Load()
        {
            Terraria.On_Player.KeyDoubleTap += ActivateSetBonus;
        }

        private void ActivateSetBonus(Terraria.On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            if (keyDir == (Main.ReversedUpDownArmorSetBonuses ? 1 : 0) && HasArmorSet(player) && !player.mount.Active)
            {
                // The set bonus can only be activated if the player does not have the cooldown.
                if (!player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out CooldownInstance cd))
                {
                    player.AddCooldown(SandsmokeBomb.ID, SmokeCooldown + SmokeDuration);
                }
            }

            orig(player, keyDir);
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1; //6
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ItemType<DesertProwlerShirt>() && legs.type == ItemType<DesertProwlerPants>();
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<DesertProwlerHat>() && player.armor[1].type == ItemType<DesertProwlerShirt>() && player.armor[2].type == ItemType<DesertProwlerPants>();
        public bool IsPartOfSet(Item item) => item.type == ItemType<DesertProwlerHat>() ||
                item.type == ItemType<DesertProwlerShirt>() ||
                item.type == ItemType<DesertProwlerPants>();


        public override void UpdateArmorSet(Player player)
        {           
            player.setBonus = this.GetLocalization("SetBonus").Format(FlatDamageBonus); //More gets edited in elsewhere
            player.GetDamage<RangedDamageClass>().Flat += FlatDamageBonus;

            DesertProwlerPlayer armorPlayer = player.GetModPlayer<DesertProwlerPlayer>();
            armorPlayer.desertProwlerSet = true;

            if (ShroudedInSmoke(player, out var cd))
            {
                if (cd.timeLeft == SmokeCooldown + SmokeDuration)
                    armorPlayer.SetBonusStartEffect();
                
                player.moveSpeed *= 1.5f;
                player.invis = true;
                player.aggro = (int)(player.aggro * 0.5f);
                player.noKnockback = true;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustDisplace = Main.rand.NextVector2Circular(80f, 50f);
                    Vector2 dustPosition = player.MountedCenter + dustDisplace;
                    Vector2 dustSpeed = Main.rand.NextVector2Circular(0.5f, 0.5f) + player.velocity / 8f - Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 0.06f;
                    dustSpeed.X += 1.4f * (float)Math.Sin(((dustDisplace.X + 80f) / 160f) * MathHelper.Pi) * (Main.rand.NextBool() ? -1 : 1);
                    Particle dust = new SandyDustParticle(dustPosition, dustSpeed, Color.White, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(20, 50), 0.03f, Vector2.UnitY * 0.03f);
                    GeneralParticleHandler.SpawnParticle(dust);
                }

                int sandSmokeCount = Main.rand.Next(2, 3);
                for (int i = 0; i < sandSmokeCount; i++)
                {
                    Color startColor = new Color(173, 156, 112);
                    Color endColor = new Color(143, 120, 63);
                    if (Main.rand.NextBool())
                    {
                        startColor = new Color(173, 139, 100);
                        endColor = new Color(149, 106, 50);
                    }

                    //Add some random hue shift & sat shift
                    Vector3 hslStartColor = Main.rgbToHsl(startColor);
                    Vector3 hslEndColor = Main.rgbToHsl(endColor);

                    float valueShift = Main.rand.NextFloat(0f, 0.5f);
                    float satShift = Main.rand.NextFloat(-0.1f, 0f);
                    float hueShiftPercent = Main.rand.NextFloat();

                    hslStartColor.Z = Math.Clamp(hslStartColor.Z + valueShift, 0f, 1f);
                    hslEndColor.Z = Math.Clamp(hslEndColor.Z + valueShift, 0f, 1f);

                    hslStartColor.Y = Math.Clamp(hslStartColor.Y + satShift, 0f, 1f);
                    hslEndColor.Y = Math.Clamp(hslEndColor.Y + satShift, 0f, 1f);

                    hslStartColor.X = MathHelper.Lerp(hslStartColor.X, 43 / 255f, hueShiftPercent);
                    hslEndColor.X = MathHelper.Lerp(hslEndColor.X, 43 / 255f, hueShiftPercent);

                    startColor = Main.hslToRgb(hslStartColor);
                    endColor = Main.hslToRgb(hslEndColor);

                    Vector2 smokeRandomPos = Main.rand.NextVector2Circular(40f, player.height);
                    Vector2 smokePos = player.MountedCenter + smokeRandomPos;
                    float burstAngle = MathHelper.Pi - ((smokeRandomPos.X + 40) / 80f) * MathHelper.Pi;
                    Vector2 smokeSpeed = Main.rand.NextVector2Circular(1f, 0.5f) - Vector2.UnitY * 0.05f + player.velocity * 0.5f + burstAngle.ToRotationVector2() * ((1 - (float)Math.Sin(burstAngle)) * 0.9f + 0.1f) * 1.5f;
                    smokeSpeed.X += (float)Math.Sin(((smokeRandomPos.X + 40) / 80f) * MathHelper.Pi) * (Main.rand.NextBool() ? -1 : 1);
                    Particle smoke = new TimedSmokeParticle(smokePos, smokeSpeed, startColor, endColor, Main.rand.NextFloat(0.7f, 1.6f), Main.rand.NextFloat(0.4f, 0.55f), Main.rand.Next(20, 36), 0.01f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }

                // Dust
                Vector2 dustDirection = Main.rand.NextVector2CircularEdge(1f, 1f);
                float dustDistance = Main.rand.NextFloat(30f);
                Vector2 dustPos = player.MountedCenter + dustDirection * dustDistance;
                int dustType = Main.rand.NextBool() ? 32 : 31;

                Dust dhusvtt = Dust.NewDustPerfect(dustPos, dustType);
                dhusvtt.noGravity = true;
                dhusvtt.fadeIn = 1f;
                Vector2 dustVelocity = dustDirection.RotatedBy(MathHelper.PiOver2) * 0.04f * dustDistance;
                dhusvtt.velocity = dustVelocity;

            }
        }

        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    string dir = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
                    TooltipLine setBonus1 = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", CalamityUtils.GetTextFromModItem<DesertProwlerHat>("AbilityBrief").Format(dir));
                    setBonus1.OverrideColor = Color.Lerp(new Color(255, 229, 156), new Color(233, 225, 198), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f));
                    tooltips.Insert(setBonusIndex + 1, setBonus1);

                    TooltipLine setBonus2 = new TooltipLine(item.Mod, "CalamityMod:SetBonus2", CalamityUtils.GetTextFromModItem<DesertProwlerHat>("AbilityDescription").Format(FreeCrit, BonusDamageCap, LightsOutReset / 60f));
                    setBonus2.OverrideColor = new Color(204, 181, 72);
                    tooltips.Insert(setBonusIndex + 2, setBonus2);
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => ModifySetTooltips(this, tooltips);

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 4;
            player.ammoCost80 = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormlionMandible>(2).
                AddIngredient(ItemID.Silk, 8).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class DesertProwlerShirt : ModItem, IBulkyArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public string BulkTexture => "CalamityMod/Items/Armor/DesertProwler/DesertProwlerShirt_Bulk";

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DesertProwlerHat.ModifySetTooltips(this, tooltips);

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormlionMandible>(3).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class DesertProwlerPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => DesertProwlerHat.ModifySetTooltips(this, tooltips);
        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormlionMandible>().
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.Loom).
                Register();
        }
    }

    public class DesertProwlerPlayer : ModPlayer
    {
        public static int BastionShootDamage = 10;
        public static float BastionShootSpeed = 18f;
        public static int BastionShootTime = 10;
        internal SlotId SmokeBombSoundSlot;

        public bool desertProwlerSet = false;
        public bool stopSmokeBomb = false;

        public override void ResetEffects()
        {
            desertProwlerSet = false;
        }

        public override void UpdateDead()
        {
            desertProwlerSet = false;
        }

        public override void PostUpdate()
        {
            if (Player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out var cd))
            {
                if (stopSmokeBomb)
                {
                    cd.timeLeft = DesertProwlerHat.SmokeCooldown;
                    SetBonusBounceEffect();
                    stopSmokeBomb = false;
                }

                if (cd.timeLeft == DesertProwlerHat.SmokeCooldown)
                SetBonusEndEffect();
            }
        }

        public void SetBonusStartEffect()
        {
            SmokeBombSoundSlot = SoundEngine.PlaySound(DesertProwlerHat.SmokeBombSound, Player.Center);
        }

        public void SetBonusEndEffect()
        {
            if (SoundEngine.TryGetActiveSound(SmokeBombSoundSlot, out var sound))
            {
                sound.Stop();
                SmokeBombSoundSlot = SlotId.Invalid;
            }
        }

        //BANDIT RISK OF RAIN 2! JAce would b e proud
        public void SetBonusBounceEffect()
        {
            SoundEngine.PlaySound(DesertProwlerHat.SmokeBombEndSound, Player.Center);
            Player.velocity.Y = Math.Min(-6f, Player.velocity.Y - 6f);
            Player.jump = Player.jumpHeight / 2;

            for (int i = 0; i < 30; i++)
            {
                Vector2 dustDisplace = Main.rand.NextVector2Circular(80f, 30f);
                Vector2 dustPosition = Player.MountedCenter + Vector2.UnitY * Player.height * 0.5f + dustDisplace;
                Vector2 dustSpeed = Main.rand.NextVector2Circular(0.5f, 0.5f) + Player.velocity / 8f - Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 0.06f;
                dustSpeed.X += 1.4f * (float)Math.Sin(((dustDisplace.X + 80f) / 160f) * MathHelper.Pi) * (Main.rand.NextBool() ? -1 : 1);
                Particle dust = new SandyDustParticle(dustPosition, dustSpeed, Color.White, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(20, 50), 0.03f, Vector2.UnitY * 0.03f);
                GeneralParticleHandler.SpawnParticle(dust);
            }


            if (!Main.dedServ)
            {
                for (int i = 0; i < 10; i++)
                {
                    float dustOrientation = i / 10f * MathHelper.TwoPi + MathHelper.PiOver4 * 0.6f;
                    Vector2 dustDirection = Vector2.UnitX * (float)Math.Sin(dustOrientation) + Vector2.UnitY * (float)Math.Cos(dustOrientation) * 0.2f;

                    Vector2 dustPos = Player.MountedCenter + Vector2.UnitY * Player.height / 2f + dustDirection * 20f;
                    Vector2 dustVel = Player.velocity * 0.3f + dustDirection * 1.4f;

                    int sandstormJump = Gore.NewGore(Player.GetSource_Misc("Jump"), dustPos, dustVel, Main.rand.Next(220, 223), 1f);
                    Main.gore[sandstormJump].velocity = dustVel;
                    Main.gore[sandstormJump].alpha = 100;


                    for (int j = 0; j < 1; j++)
                    {
                        Dust miniDust = Dust.NewDustDirect(dustPos, 32, 32, 124, dustVel.X, dustVel.Y * 0.3f, 150, default(Color), 1f);
                        miniDust.fadeIn = 1.5f;
                    }
                }
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (!desertProwlerSet && DesertProwlerHat.ShroudedInSmoke(Player, out var cd))
            {
                cd.timeLeft = DesertProwlerHat.SmokeCooldown;
            }
        }
    }

    public class DesertProwlerProjectile : GlobalProjectile
    {
        public bool LightsOut = false;
        public int ExtraCrit = 0;
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.damage > 0 && !Main.gameMenu)
            {
                if (projectile.owner >= 0 && DesertProwlerHat.ShroudedInSmoke(Main.player[projectile.owner], out var cd) && projectile.DamageType.CountsAsClass(DamageClass.Ranged))
                {
                    int critPool = DesertProwlerHat.FreeCrit;

                    int achievedDamage = projectile.damage;

                    //Increase the crit cance of the projectile for as long as theres free crits to be handed out.
                    //Only increase the crit chance if the additional 100% damage wouldnt make the projectile deal more than the damage cap
                    while (critPool >= 100)
                    {
                        if (achievedDamage + projectile.damage <= DesertProwlerHat.BonusDamageCap)
                        {
                            ExtraCrit += 100;
                            critPool -= 100;

                            achievedDamage += projectile.damage;
                        }

                        else
                        {
                            //Don't do anything to projectiles that reached the damage cap
                            if (achievedDamage < DesertProwlerHat.BonusDamageCap)
                            {
                                //Give some compensation crit if there's still some crit left but adding one full 100% crit chance would make the projectile go over the damage cap
                                //For example, if the free crit was 200%, the projectile dealt 40 damage, and the damage cap was 100.
                                //The projectile would get one layer of extra crit, bringing it to 80 damage, but we still have 100% crit leftover
                                //Adding an extra layer of crit would make the projectile deal 120 damage, which we do not want.
                                //To compensate for the 20 potential damage loss, we instead add 20/40 crit, making it have 50% extra crit

                                int remainingDamageTilCap = DesertProwlerHat.BonusDamageCap - achievedDamage;
                                ExtraCrit += (int)(100 * remainingDamageTilCap / (float)projectile.damage);
                            }

                            //Avoid infinite loops
                            break;
                        }
                    }

                    projectile.CritChance += ExtraCrit;

                    projectile.Calamity().supercritHits  = 1;
                    LightsOut = true;
                    Main.player[projectile.owner].GetModPlayer<DesertProwlerPlayer>().stopSmokeBomb = true;
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (LightsOut)
            {
                Player owner = Main.player[projectile.owner];
                projectile.CritChance -= ExtraCrit;

                if (target.life <= 0 && owner.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out var cd) && cd.timeLeft <= DesertProwlerHat.SmokeCooldown && cd.timeLeft > DesertProwlerHat.LightsOutReset)
                {
                    cd.timeLeft = DesertProwlerHat.LightsOutReset;
                    SoundEngine.PlaySound(DesertProwlerHat.CDResetSound);

                    Particle skully = new DesertProwlerSkullParticle(target.Center, Vector2.UnitY * -3f, Color.Gold, Color.DarkGoldenrod, Main.rand.NextFloat(1f, 2f), 250f);
                    GeneralParticleHandler.SpawnParticle(skully);
                }

                LightsOut = false;
            }
        }
    }
}
