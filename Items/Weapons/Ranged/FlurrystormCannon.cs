using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlurrystormCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static int AmmoSavedPercent = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent);
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn, BuffID.Frozen];
        }

        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 38;
            Item.damage = 10;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.2f;

            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlurrystormCannonShooting>();
            Item.useAmmo = AmmoID.Snowball;
            Item.shootSpeed = 18f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0 && Main.rand.Next(100) >= AmmoSavedPercent;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => type = Item.shoot;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SnowballCannon).
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.Bone, 10).
                AddIngredient<PearlShard>(10).
                AddTile(TileID.Anvils).
                AddCondition(Condition.NotRemixWorld).
                Register();

            // CIT 16NOV2024: Due to Snowball Cannon being swapped with Ice Bow in Remix, Flurrystorm Cannon uses Ice Bow in its recipe there.
            // Yes, this makes no sense. I don't care; I prefer obtainability over making sense.
            CreateRecipe().
                AddIngredient(ItemID.IceBow).
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.Bone, 10).
                AddIngredient<PearlShard>(10).
                AddTile(TileID.Anvils).
                AddCondition(Condition.RemixWorld).
                Register();
        }
    }
}
