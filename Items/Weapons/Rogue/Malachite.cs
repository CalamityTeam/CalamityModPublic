using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    // Have to directly implement the interface instead of extending LegendaryItem since it already extends RogueWeapon
    public class Malachite : RogueWeapon, IHoldShiftTooltipItem
    {
        public string ExtensionIndicatorKey => "Items.Misc.LegendaryShortTooltip";
        public Color? ExtensionIndicatorColor => null;
        public string TooltipExtensionKey => "LegendaryText";
        public Color? TooltipExtensionColor => Color.Lime;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Plague>()];
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 58;
            Item.damage = 52;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MalachiteProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                Item.UseSound = SoundID.Item109;
                Item.shoot = ModContent.ProjectileType<MalachiteStealth>();
            }
            else if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item109;
                Item.shoot = ModContent.ProjectileType<MalachiteBolt>();
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                Item.shoot = ModContent.ProjectileType<MalachiteProj>();
            }
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse == 2)
                return 1f;
            return 2f;
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2 && !player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.75f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (float i = -6.5f; i <= 6.5f; i += 6.5f)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                    int stealth = Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }
    }
}
