using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("ElementalBlaster")]
    public class SuperradiantSlaughterer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public const float ShootSpeed = 24f;
        public const int DashCooldown = 360;

        public bool hasDashed = false;
        public int rightClickDelay = 0;
        public static int doubleRightFrameWindow = 23;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips == null)
                return;

            Player player = Main.LocalPlayer;
            if (player is null)
                return;

            var mainTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[MAIN]") && x.Mod == "Terraria");
            if (mainTooltip != null)
            {
                mainTooltip.Text = Lang.SupportGlyphs(this.GetLocalizedValue("MainInfo"));
                mainTooltip.OverrideColor = Color.Chartreuse;
            }
            var altTooltip = tooltips.FirstOrDefault(x => x.Text.Contains("[ALT]") && x.Mod == "Terraria");
            if (altTooltip != null)
            {
                altTooltip.Text = Lang.SupportGlyphs(this.GetLocalization("AltInfo").Format(DashCooldown / 60));
                altTooltip.OverrideColor = Color.SpringGreen;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ElementalMix>(), ModContent.BuffType<Laceration>()];
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 46;
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 21;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 1.75f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SuperradiantSlaughtererHoldout>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            player.Calamity().mouseWorldListener = true;
            player.Calamity().rightClickListener = true;

            if (rightClickDelay > 0)
                rightClickDelay--;
            if (!player.HasCooldown(SuperradiantSawBoost.ID))
                hasDashed = false;
            
            // Right-click channeling
            if (player.Calamity().mouseRight && CanUseItem(player) && !Main.mapFullscreen && !Main.blockMouse && !player.HasCooldown(SuperradiantSawBoost.ID))
            {
                // Only one out at a time
                if (Main.projectile.Any(n => n.active && n.type == Item.shoot && n.owner == player.whoAmI))
                    return;

                int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage);
                float kb = player.GetTotalKnockback<MeleeDamageClass>().ApplyTo(Item.knockBack);

                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, player.SafeDirectionTo(player.Calamity().mouseWorld), Item.shoot, damage * 2, kb, player.whoAmI, ai1: 2f);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Handles allowing the dash
            if (player.altFunctionUse == 2)
            {
                bool doubleRightClick = rightClickDelay > 0 && !hasDashed;
                bool canDash = doubleRightClick && player.Calamity().DashID != SuperradiantSawDash.ID;

                // If you hit the right-click window, dash
                if (canDash)
                {
                    hasDashed = true;
                    player.Calamity().sBlasterDashActivated = true;
                }

                // Set the double right-click frames
                rightClickDelay = doubleRightFrameWindow;
                // The holdout will initially double up when right clicking otherwise
                return false;
            }

            // The holdout deals 1.5x base damage.
            Projectile.NewProjectile(source, position, velocity, Item.shoot, (int)(damage * 1.5), knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Buzzkill>().
                AddIngredient<SpeedBlaster>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
