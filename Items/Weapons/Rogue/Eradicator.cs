using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Eradicator : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
            Tooltip.SetDefault("Throws a disk that fires lasers at nearby enemies\n" +
			"Stealth strikes stick to enemies and unleash a barrage of lasers in all directions");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 300;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 19;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.height = 54;
			item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.DarkBlue;
			item.shoot = ModContent.ProjectileType<EradicatorProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
			if (player.Calamity().StealthStrikeAvailable())
			{
				Main.projectile[proj].Calamity().stealthStrike = true;
				Main.projectile[proj].timeLeft *= 2;
			}
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/EradicatorGlow"));
        }
    }
}
