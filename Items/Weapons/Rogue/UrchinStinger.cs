using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class UrchinStinger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Stinger");
            Tooltip.SetDefault("Stealth strikes stick to enemies while releasing sulphuric bubbles");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.damage = 13;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<UrchinStingerProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<UrchinStingerProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[proj].Calamity().stealthStrike = true;
            return false;
        }
    }
}
