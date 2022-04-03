using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IonBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ion Blaster");
            Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
                "Damage scales with how full your mana is\n" +
                "Using Astral Injection reduces the effectiveness of the mana boost");
        }

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.width = 44;
            Item.height = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item91;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<IonBlast>();
            Item.shootSpeed = 3f;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float manaRatio = (float)player.statMana / player.statManaMax2;
            bool injectionNerf = player.Calamity().astralInjection;
            if (injectionNerf)
                manaRatio = MathHelper.Min(manaRatio, 0.65f);

            // 20% to 160% damage. Astral Injection caps it at 111% damage.
            float damageRatio = 0.2f + 1.4f * manaRatio;
            int finalDamage = (int)(damage * damageRatio);

            Projectile proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, finalDamage, knockBack, player.whoAmI);
            proj.scale = 0.75f + 0.75f * manaRatio;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SoulofFright, 10).AddRecipeGroup("AnyAdamantiteBar", 7).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
