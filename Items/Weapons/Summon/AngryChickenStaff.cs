using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AngryChickenStaff : ModItem
    {
        public const int Damage = 107;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharon's Kindle Staff");
            Tooltip.SetDefault("Summons the Son of Yharon to fight for you\n" +
                               "Requires 4 minion slots to use");
        }

        public override void SetDefaults()
        {
            item.mana = 10;
            item.damage = Damage;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 32;
            item.height = 32;
            item.useTime = item.useAnimation = 10;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SonOfYharon>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
