using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BloodBoiler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Boiler");
            Tooltip.SetDefault("Fires a stream of lifestealing bloodfire\n" +
                "Uses your health as ammo\n" + "25% chance to not consume ammo");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 30;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<BloodBoilerFire>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextFloat() > 0.75f)
                --player.statLife;
            if (player.statLife <= 0)
            {
                PlayerDeathReason pdr = PlayerDeathReason.ByCustomReason(Main.rand.NextBool(2) ? player.name + " suffered from severe anemia." : player.name + " was unable to obtain a blood transfusion.");
                player.KillMe(pdr, 1000.0, 0, false);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BloodstoneCore>(), 6).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
