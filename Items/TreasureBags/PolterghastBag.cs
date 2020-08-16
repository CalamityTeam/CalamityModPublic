using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PolterghastBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Polterghast>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(16f, 20f); //16, 17
			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/TreasureBags/PolterghastBagGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<RuinousSoul>(), 10, 20);
            DropHelper.DropItem(player, ModContent.ItemType<Phantoplasm>(), 20, 30);

            // Weapons
			DropHelper.DropWeaponSet(player, 3,
				ModContent.ItemType<TerrorBlade>(),
				ModContent.ItemType<BansheeHook>(),
				ModContent.ItemType<DaemonsFlame>(),
				ModContent.ItemType<FatesReveal>(),
				ModContent.ItemType<GhastlyVisage>(),
				ModContent.ItemType<EtherealSubjugator>(),
				ModContent.ItemType<GhoulishGouger>());

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<Affliction>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<Ectoheart>(), CalamityWorld.revenge && !player.Calamity().adrenalineBoostThree);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PolterghastMask>(), 7);
        }
    }
}
