using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ApoctosisArray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apoctosis Array");
            Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
                "Damage scales with how full your mana is\n" +
                "Using Astral Injection reduces the effectiveness of the mana boost");
        }

        public override void SetDefaults()
        {
            Item.damage = 99;
            Item.width = 98;
            Item.height = 34;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useAnimation = 7;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item91;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<IonBlast>();
            Item.shootSpeed = 8f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            float manaRatio = (float)player.statMana / player.statManaMax2;
            bool injectionNerf = player.Calamity().astralInjection;
            if (injectionNerf)
                manaRatio = MathHelper.Min(manaRatio, 0.65f);

            // 20% to 160% damage. Astral Injection caps it at 111% damage.
            float damageRatio = 0.2f + 1.4f * manaRatio;
            damage = (int)(damage * damageRatio);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float manaRatio = (float)player.statMana / player.statManaMax2;
            bool injectionNerf = player.Calamity().astralInjection;
            if (injectionNerf)
                manaRatio = MathHelper.Min(manaRatio, 0.65f);

            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.scale = 0.75f + 0.75f * manaRatio;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<IonBlaster>()).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
