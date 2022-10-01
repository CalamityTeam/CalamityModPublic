using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AbyssalTome : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Tome");
            Tooltip.SetDefault("Casts a slow-moving ball of dark energy");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SlimeGodCore.ShotSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AbyssBall>();
            Item.shootSpeed = 9f;
        }
    }
}
