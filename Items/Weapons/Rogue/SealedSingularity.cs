using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SealedSingularity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sealed Singularity");
            Tooltip.SetDefault("Shatters on impact, summoning a black hole that sucks in nearby enemies\n" +
            "Stealth strikes summon a black hole that lasts longer and sucks enemies with stronger force");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 260;
            Item.knockBack = 5f;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SealedSingularityProj>();
            Item.shootSpeed = 14f;

            Item.noMelee = Item.noUseGraphic = true;
            Item.height = Item.width = 34;
            Item.UseSound = SoundID.Item106;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                // Directly nerf stealth strikes by 32%, but only stealth strikes.
                int stealthDamage = (int)(damage * 0.72f);
                int stealth = Projectile.NewProjectile(source, position, velocity, type, stealthDamage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DuststormInABottle>().
                AddIngredient<DarkPlasma>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
