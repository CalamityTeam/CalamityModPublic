using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CindersOfLament : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 1666;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 86;
            Item.height = 92;
            Item.useTime = Item.useAnimation = 30; // 29 because of useStyle 1
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.shoot = ModContent.ProjectileType<CataclysmSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<EntropysVigil>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (Main.rand.NextBool())
                    type = ModContent.ProjectileType<CatastropheSummon>();
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
