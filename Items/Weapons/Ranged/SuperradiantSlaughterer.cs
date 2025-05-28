using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("ElementalBlaster")]
    public class SuperradiantSlaughterer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public const float ShootSpeed = 24f;

        public const int DashCooldown = 360;

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
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 46;
            Item.damage = 127;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
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
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 21;
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.HasCooldown(SuperradiantSawBoost.ID))
                return false;
            else
                return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            player.Calamity().mouseWorldListener = true;
            player.Calamity().rightClickListener = true;

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
            // The holdout will initially double up when right clicking otherwise
            if (player.altFunctionUse == 2)
                return false;

            // The holdout deals 2x base damage.
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage * 2, knockback, player.whoAmI);
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
