using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AdamantiteParticleAccelerator : ModItem
    {
        public const int ChargeFrames = 28;
        public const int CooldownFrames = 16;
        public static readonly Color[] LightColors = new Color[] { new Color(235, 40, 121), new Color(49, 161, 246) }; //beam colors
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Particle Accelerator");
            Tooltip.SetDefault("Charges and fires 2 beams of opposite polarities. Hitting with the opposite polarity increases the damage");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 52;
            Item.useTime = ChargeFrames + CooldownFrames;
            Item.useAnimation = ChargeFrames + CooldownFrames;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<AdamantiteAcceleratorHoldout>();
            Item.noUseGraphic = true;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity *= Item.shootSpeed;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AdamantiteAcceleratorHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteBar, 10).
                AddIngredient(ItemID.SoulofLight, 3).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
