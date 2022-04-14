using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Norfleet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Norfleet");
            Tooltip.SetDefault("Fire everything!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 354;
            Item.knockBack = 15f;
            Item.shootSpeed = 30f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 75;
            Item.useTime = 75;
            Item.reuseDelay = 0;
            Item.width = 140;
            Item.height = 42;
            Item.UseSound = SoundID.Item92;
            Item.shoot = ModContent.ProjectileType<NorfleetCannon>();
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.useTurn = false;
            Item.useAmmo = AmmoID.FallenStar;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<NorfleetCannon>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
}
