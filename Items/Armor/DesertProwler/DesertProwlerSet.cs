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

namespace CalamityMod.Items.Armor.DesertProwler
{
    [AutoloadEquip(EquipType.Head)]
    public class DesertProwlerHat : ModItem
    {
        public static int SmokeCooldown = 25 * 60;
        public static int SmokeDuration = 5 * 60;

        public static bool ShroudedInSmoke(Player player, out CooldownInstance cd)
        {
            cd = null;
            bool hasSmokebombCD = player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out cd);
            return (hasSmokebombCD && cd.timeLeft > SmokeCooldown);
        }

        public override void Load()
        {
            On.Terraria.Player.KeyDoubleTap += ActivateSetBonus;
        }

        public override void Unload()
        {
            On.Terraria.Player.KeyDoubleTap -= ActivateSetBonus;
        }

        private void ActivateSetBonus(On.Terraria.Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            if (keyDir == 0 && HasArmorSet(player) && !player.mount.Active)
            {
                //Only activate if no cooldown & available scrap.
                if (!player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out CooldownInstance cd))
                {
                    player.AddCooldown(SandsmokeBomb.ID, SmokeCooldown + SmokeDuration);
                }
            }

            orig(player, keyDir);
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Hat");
            Tooltip.SetDefault("4% increased ranged critical strike chance and 20% chance to not consume ammo");
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
            player.setBonus = "Ranged attacks deal an extra 1 flat damage"; //More gets edited in elsewhere
            
            DesertProwlerPlayer armorPlayer = player.GetModPlayer<DesertProwlerPlayer>();
            armorPlayer.desertProwlerSet = true;

            if (ShroudedInSmoke(player, out var cd))
            {
                if (cd.timeLeft == SmokeCooldown + SmokeDuration)
                {
                    //Vfx and noises
                }

                player.moveSpeed *= 1.5f;
                player.invis = true;
                player.GetCritChance(DamageClass.Ranged) += 300;

                //Visuals
                Vector2 smokePos = player.MountedCenter + Main.rand.NextVector2Square(-player.height / 2, player.height / 2);

                /*
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.MountedCenter, 0, 0, DustType<GasDust>());
                    dust.position += Main.rand.NextVector2Square(-player.height / 2, player.height / 2);
                    dust.velocity = Main.rand.NextVector2Circular(1, 1) + player.velocity / 2;
                    dust.scale = 2.4f;
                    dust.color = new Color(217, 154, 154);
                }
                */

                for (int i = 0; i < 15; i++)
                {
                    Particle dust = new SandyDustParticle(player.MountedCenter + Main.rand.NextVector2Square(-player.height * 0.8f, player.height * 0.8f), Main.rand.NextVector2Circular(1, 1) + player.velocity / 2, Color.White, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(10, 40), rotationSpeed:0.03f);
                    GeneralParticleHandler.SpawnParticle(dust);
                }
                

                
                Color startColor = new Color(173, 156, 112);
                Color endColor = new Color(143, 120, 63);
                if (Main.rand.NextBool())
                {
                    startColor = new Color(173, 139, 100);
                    endColor = new Color(149, 106, 50);
                }

                Particle smoke = new TimedSmokeParticle(smokePos, Main.rand.NextVector2Circular(2f, 1f) - Vector2.UnitY * 1f + player.velocity * 0.5f, startColor, endColor, Main.rand.NextFloat(0.7f, 2.2f), Main.rand.NextFloat(0.6f, 0.85f), Main.rand.Next(40, 76), 0.01f);
                GeneralParticleHandler.SpawnParticle(smoke);
                

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
                    TooltipLine setBonus1 = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", "Sandsmoke Bomb - Double tap DOWN to shroud yourself in a small cloud of sand");
                    setBonus1.OverrideColor = Color.Lerp(new Color(255, 229, 156), new Color(233, 225, 198), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f));
                    tooltips.Insert(setBonusIndex + 1, setBonus1);

                    TooltipLine setBonus2 = new TooltipLine(item.Mod, "CalamityMod:SetBonus2", "While the sand cloud is active, gain increased mobility");
                    setBonus2.OverrideColor = new Color(204, 181, 72);
                    tooltips.Insert(setBonusIndex + 2, setBonus2);

                    TooltipLine setBonus3 = new TooltipLine(item.Mod, "CalamityMod:SetBonus3", "Attacking instantly dispels the sand cloak, but guarantees a supercrit for 200% damage");
                    setBonus3.OverrideColor = new Color(204, 181, 72);
                    tooltips.Insert(setBonusIndex + 3, setBonus3);
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
                AddIngredient<DesertFeather>(2).
                AddIngredient(ItemID.Silk, 8).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class DesertProwlerShirt : ModItem, IBulkyArmor
    {
        public string BulkTexture => "CalamityMod/Items/Armor/DesertProwler/DesertProwlerShirt_Bulk";

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Shirt");
            Tooltip.SetDefault("5% increased ranged critical strike chance");

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
                AddIngredient<DesertFeather>(3).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.Loom).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class DesertProwlerPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Desert Prowler Pants");
            Tooltip.SetDefault("10% increased movement speed and immunity to the Mighty Wind debuff");
        }

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
                AddIngredient<DesertFeather>().
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

        public bool desertProwlerSet = false;

        public override void ResetEffects()
        {
            desertProwlerSet = false;
        }

        public override void UpdateDead()
        {
            desertProwlerSet = false;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            // Prismatic Breaker is a weird hybrid melee-ranged weapon so include it too.  Why are you using desert prowler post-Yharon? don't ask me
            if (desertProwlerSet && (item.CountsAsClass<RangedDamageClass>() || item.type == ModContent.ItemType<PrismaticBreaker>()) && item.ammo == AmmoID.None)
                damage.Flat += 1f;
        }

        public override void PostUpdate()
        {
            if (Player.Calamity().cooldowns.TryGetValue(SandsmokeBomb.ID, out var cd) && cd.timeLeft == DesertProwlerHat.SmokeCooldown)
            {
                SetBonusEndEffect();
            }
        }

        public void SetBonusEndEffect()
        {
            //SoundStyle endSound = DesertProwlerHat.SmokeBombEndSound;
            //Dust
        }

        public override void PostUpdateMiscEffects()
        {
            //This shouldn't ever be possible since the power mode prevents you from using or moving items around
            if (!desertProwlerSet && DesertProwlerHat.ShroudedInSmoke(Player, out var cd))
            {
                cd.timeLeft = DesertProwlerHat.SmokeCooldown;
            }
        }
    }

    public class DesertProwlerProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            DesertProwlerHat.SmokeCooldown = 20;

            if (projectile.damage > 0)
            {
                if (projectile.owner >= 0 && DesertProwlerHat.ShroudedInSmoke(Main.player[projectile.owner], out var cd))
                {
                    projectile.Calamity().canSupercrit = true;
                    cd.timeLeft = DesertProwlerHat.SmokeCooldown;
                }
            }
        }
    }
}
